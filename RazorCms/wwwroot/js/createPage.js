document.addEventListener('DOMContentLoaded', () => {
    document.querySelectorAll('[data-add-block]').forEach(btn => {
        btn.addEventListener('click', () => addBlock(btn.dataset.addBlock));
    });
    document.querySelector('#saveBtn').addEventListener('click', savePage);
});

const editor = document.getElementById('editor');
const blocks = [];
let count = 0;
// store the dragged item-block
let draggedBlock;

function addBlock(type) {
    //init the block obj with element type and order num
    const block = { type, order: count++ };
    const wrapper = document.createElement('div');
    wrapper.className = "mb-2 p-2 border";
    wrapper.draggable = true;
    // essential to know the old order after dropping and chagning order
    wrapper.dataset.order = block.order;

    wrapper.addEventListener('dragstart', () => {
        draggedBlock = wrapper;
    });

    wrapper.addEventListener('dragover', (e) => {
        e.preventDefault();
    });

    wrapper.addEventListener('drop', (e) => {
        e.preventDefault();
        if (draggedBlock && draggedBlock !== wrapper) {
            // Array.from() returns an array from a collection
            // such as htmlElementCollection (editior.children)
            const draggedIndex = Array.from(editor.children).indexOf(draggedBlock);
            const targetIndex = Array.from(editor.children).indexOf(wrapper);

            if (draggedIndex < targetIndex) {
                editor.insertBefore(draggedBlock, wrapper.nextSibling);
            } else {
                editor.insertBefore(draggedBlock, wrapper);
            }

            updateOrder();
        }
    });

    if (type === 'text') {
        block.text = '';
        const textarea = document.createElement('textarea');
        textarea.className = "form-control";
        textarea.placeholder = "Text block...";
        textarea.oninput = () => block.text = textarea.value;
        wrapper.appendChild(textarea);
    } else if (type === 'header') {
        block.text = '';
        const input = document.createElement('input');
        input.className = "form-control";
        input.style.fontSize = '20px';
        input.placeholder = "Header text...";
        input.oninput = () => block.text = input.value;
        wrapper.appendChild(input);

    } else if (type === 'image') {
        block.url = '';
        const input = document.createElement('input');
        input.className = "form-control";
        input.placeholder = "Image URL...";
        input.oninput = () => block.url = input.value;
        wrapper.appendChild(input);
    } else if (type === 'link') {
        block.url = '';
        block.text = '';
        const urlInput = document.createElement('input');
        urlInput.className = "form-control mb-2";
        urlInput.placeholder = "Link URL...";
        urlInput.oninput = () => block.url = urlInput.value;
        wrapper.appendChild(urlInput);
        const textInput = document.createElement('input');
        textInput.className = "form-control";
        textInput.placeholder = "Link text...";
        textInput.oninput = () => block.text = textInput.value;
        wrapper.appendChild(textInput);
    }

    const removeBtn = document.createElement('button');
    removeBtn.innerText = "Remove";
    removeBtn.className = "btn btn-danger btn-sm mt-2";
    removeBtn.onclick = () => {
        editor.removeChild(wrapper);
        const index = blocks.findIndex(b => b.order == block.order);
        if (index > -1) blocks.splice(index, 1);
        updateOrder();
    };
    wrapper.appendChild(removeBtn);

    editor.appendChild(wrapper);
    blocks.push(block);
    // Dispatch custom event after block is added
    const event = new CustomEvent('blockAdded', { detail: { block } });
    document.dispatchEvent(event);

}

function updateOrder() {
    const children = Array.from(editor.children);
    children.forEach((child, i) => {
        const oldOrder = Number(child.dataset.order); // store before overwriting
        const block = blocks.find(b => b.order === oldOrder);
        if (block) {
            block.order = i;
            child.dataset.order = i;
        }
    });
}


async function savePage() {
    const payload = {
        title: document.getElementById('page-title').value,
        isHidden: document.getElementById('isPageHidden').checked,
        blocks: Array.from(editor.children).map(child => {
            const order = parseInt(child.dataset.order);
            return blocks.find(b => b.order == order);
        }),
        userId: document.getElementById('userId').value
    };

    const res = await fetch('https://localhost:7170/api/pages/save/', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload)
    });

    if (res.ok) {
        alert("Page saved!");
        location.reload();
    } else {
        alert("Failed to save page");
        console.log(payload);
    }
}


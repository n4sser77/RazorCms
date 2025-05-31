
document.querySelectorAll('[data-add-block]').forEach(btn => {
    btn.addEventListener('click', () => addBlock(btn.dataset.addBlock));
});
document.querySelector('#saveBtn').addEventListener('click', saveChanges);


const pageId = document.getElementById('page-meta').dataset.pageId;
let page;
const pageReq = fetch(`https://localhost:7170/api/pages/${pageId}`)
    .then(res => {
        if (!res.ok) throw new Error(`HTTP error! status: ${res.status}`);
        return res.json();
    })
    .then(data => {
        console.log(data);
        page = data;
    })
    .catch(error => console.error('Fetch error:', error));





const editor = document.querySelector('#editor');
const wrappers = document.querySelectorAll('.wrappers');


const editedBlocks = [];
const deletedBlockIds = [];
const addedBlocks = [];
let count = wrappers.length;
console.log(count)
// store the dragged item-block
let draggedBlock;



function onEditBlock(blockId, newBlock) {

    let updatedblock = page.blocks.find(b => b.id === blockId);
    if (!updatedblock) {

        updatedblock = {
            id: blockId,
            ...newBlock
        };
    }


    editedBlocks.push({
        id: blockId,
        ...newBlock
    });
}


wrappers.forEach((wrapper) => {

    const editBtn = wrapper.querySelector(".edit-btn");
    const deleteBtn = wrapper.querySelector(".delete-btn");
    if (!editBtn || !deleteBtn) {
        return;
    }
    wrapper.draggable = true;
    wrapper.addEventListener('dragstart', () => {
        draggedBlock = wrapper;
    });

    wrapper.addEventListener('dragover', (e) => {
        e.preventDefault();
    });

    wrapper.addEventListener('drop', (e) => {
        e.preventDefault();
        console.log('Before move:', page.blocks.map(b => b.id));
        if (draggedBlock && draggedBlock !== wrapper) {
            //const draggedIndex = Array.from(editor.children).indexOf(draggedBlock);
            //const targetIndex = Array.from(editor.children).indexOf(wrapper);

            const draggedId = draggedBlock.dataset.blockId;
            const targetId = wrapper.dataset.blockId;

            const draggedIndex = page.blocks.findIndex(b => b.id === draggedId);
            const targetIndex = page.blocks.findIndex(b => b.id === targetId);

            if (draggedIndex < targetIndex) {
                editor.insertBefore(draggedBlock, wrapper.nextSibling);
            } else {
                editor.insertBefore(draggedBlock, wrapper);
            }




            const movedBlock = page.blocks.splice(draggedIndex, 1)[0];
            page.blocks.splice(targetIndex, 0, movedBlock);
            // clean empty items

            page.blocks.forEach((b, index) => {
                //if undefiend or null, remove it
                if (!b || !b.id) {
                    page.blocks.splice(index, 1);
                } else {
                    b.order = index; // update order
                }
            })

            console.log('After move:', page.blocks.map(b => b.id));


        }
    })

    // add onclick to btns to handle the btn clicks
    // here
    editBtn.onclick = () => {

        const wrapper = editBtn.closest('.wrappers');
        const blockId = editBtn.dataset.blockId;
        // Find the content element (e.g., p, h2, img)
        let contentElem = wrapper.querySelector('p, h2, img');
        const tags = contentElem.tagName.toLowerCase()
        let blockType;
        if (tags === 'p' || tags === 'text') blockType = 'text';
        if (tags === 'h2' || tags === 'header') blockType = 'header';
        if (tags === 'image' || tags === 'img') blockType = 'image';
        /*let blockType = contentElem.tagName.toLowerCase();*/

        // Only allow one edit at a time per block
        if (wrapper.querySelector('.edit-controls')) return;

        let input;
        if (blockType === 'text' || blockType === 'header') {
            input = document.createElement(blockType === 'text' ? 'textarea' : 'input');
            input.className = "form-control mb-2";
            input.value = contentElem.textContent;
        } else if (blockType === 'image') {
            input = document.createElement('input');
            input.type = 'text';
            input.className = "form-control mb-2";
            input.value = contentElem.src;
        }

        // Replace content with input
        contentElem.replaceWith(input);

        // Add Save/Cancel controls
        const controls = document.createElement('div');
        controls.className = 'edit-controls mt-2';
        controls.innerHTML = `
    <button class="btn btn-success btn-sm me-2">Save</button>
    <button class="btn btn-secondary btn-sm">Cancel</button>
    `;
        input.after(controls);

        // Save handler
        controls.querySelector('.btn-success').onclick = () => {
            let newBlock = {};
            if (blockType === 'text' || blockType === 'header') {
                newBlock.text = input.value;
            } else if (blockType === 'image') {
                newBlock.url = input.value;
            }
            // Update DOM
            let newElem;
            if (blockType === 'image') {
                newElem = document.createElement('img');
                newElem.src = newBlock.url;
                newElem.className = "img-fluid mb-1";


            } else if (blockType === 'header') {
                newElem = document.createElement('h2');
                newElem.textContent = newBlock.text;
            } else if (blockType === 'text') {
                newElem = document.createElement('p');
                newElem.textContent = newBlock.text;
            }

            input.replaceWith(newElem);
            controls.remove();

            // Track the edit for API
            onEditBlock(blockId, { type: blockType, ...newBlock });
        };

        // Cancel handler
        controls.querySelector('.btn-secondary').onclick = () => {
            input.replaceWith(contentElem);
            controls.remove();
        };
    }

    deleteBtn.onclick = () => {
        const confirmed = confirm("Are you sure?")
        if (!confirmed) {
            return;
        }

        //get block id to remove
        const blockId = deleteBtn.dataset.blockId;
        if (blockId) {
            deletedBlockIds.push(blockId);
            deleteBtn.closest('.wrappers').remove();
            page.blocks = page.blocks.filter(b => b.id !== blockId);

        }

    }

    wrapper.addEventListener('mouseenter', () => {
        editBtn.classList.remove('d-none');
        deleteBtn.classList.remove('d-none');

    });

    wrapper.addEventListener('mouseleave', () => {
        editBtn.classList.add('d-none');
        deleteBtn.classList.add('d-none');
    })


});
function addBlock(type) {
    //init the block obj with element type and order num
    const block = { type, order: count++ };
    const wrapper = document.createElement('div');
    wrapper.className = "mb-2 p-2 border";



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
    }

    const removeBtn = document.createElement('button');
    removeBtn.innerText = "Remove";
    removeBtn.className = "btn btn-danger btn-sm mt-2";
    removeBtn.onclick = () => {
        editor.removeChild(wrapper);
        const index = addedBlocks.findIndex(b => b.order == block.order);
        if (index > -1) addedBlocks.splice(index, 1);

    };
    wrapper.appendChild(removeBtn);

    editor.appendChild(wrapper);
    addedBlocks.push(block);
    page.blocks.push(block);


}






async function saveChanges() {
    console.log('saving...')
    const payload = {
        pageId: document.getElementById('page-meta').dataset.pageId,
        page: page,
        editedBlocks: editedBlocks,
        deletedBlockIds: deletedBlockIds,
        addedBlocks: addedBlocks
    };
    console.log(payload);

    const res = await fetch('https://localhost:7170/api/pages/save/', {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload)
    });

    if (res.ok) {
        alert("Changes saved successfully!");
        editedBlocks.length = 0;
        deletedBlockIds.length = 0;
    } else {
        alert("Failed to save changes. Please try again.");
    }
}



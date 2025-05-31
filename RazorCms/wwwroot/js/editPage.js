console.log("Loaded");

const editedBlocks = [];
const deletedBlockIds = [];
const addedBlocks = [];

function onEditBlock(blockId, newBlock) {
    if (!blockId) {
        addedBlocks.push(newBlock);
    }
    editedBlocks.push({
        id: blockId,
        ...newBlock
    });
}

const wrappers = document.querySelectorAll('.wrappers');
const saveBtn = document.getElementById('saveBtn');

saveBtn.onclick = saveChanges;

// Find the content element (e.g., p, h2, img)

wrappers.forEach((wrapper) => {
    const deleteBtn = wrapper.querySelector(".delete-btn");
    let contentElem = wrapper.querySelector('p, h2, img');
    let blockType = contentElem.tagName.toLowerCase();

    let input;
    if (blockType === 'p' || blockType === 'h2') {
        input = document.createElement(blockType === 'p' ? 'textarea' : 'input');
        input.className = "form-control mb-2";
        input.value = contentElem.textContent;
    } else if (blockType === 'img') {
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
        if (blockType === 'p' || blockType === 'h2') {
            newBlock.text = input.value;
        } else if (blockType === 'img') {
            newBlock.url = input.value;
        }
        // Update DOM
        const newElem = document.createElement(blockType);
        if (blockType === 'img') {
            newElem.src = newBlock.url;
            newElem.className = "img-fluid mb-1";
        } else {
            newElem.textContent = newBlock.text;
        }
        const blockId = input.closest('.wrappers').dataset.blockId;
        input.replaceWith(newElem);
        controls.remove();

        // Track the edit for API
        onEditBlock(blockId, { type: blockType === 'h2' ? 'header' : blockType, ...newBlock });
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
        }
        deleteBtn.closest('.wrappers').remove();

    }


})

async function saveChanges() {


    console.log('saving...')
    const payload = {
        pageId: document.getElementById('page-meta').dataset.pageId,
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


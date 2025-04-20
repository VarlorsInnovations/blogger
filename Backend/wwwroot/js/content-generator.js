
const container = document.getElementById("content-creation-container");
const createPostSpace = document.getElementById("post-space");
const createElementBtn = document.getElementById("create-element-button");
const button = document.getElementById("create-post-button");

const template = `
<button class="remove-button" type="button">Remove</button>
<select class="part-type-selection">
            
</select><br>
<textarea  class="content-input" placeholder="Content">

</textarea>
<input type="text" id="link-input" disabled placeholder="link"/>
`

const partTypes = [
    "Heading1",
    "Heading2",
    "Heading3",
    "Heading4",
    "Paragraph",
    "Image",
    "Video",
    "Link"
] 

async function createElement() {
    const element = document.createElement("div");
    element.classList.add("part-wrapper");
    element.style.border = "1px solid black";
    
    const parsedDocument = new DOMParser().parseFromString(template, "text/html");
    element.innerHTML = parsedDocument.body.innerHTML;
    
    const partTypeSelection = element.querySelector("select");
    const removeButton = element.querySelector(".remove-button");
    
    for (const part of partTypes) {
        const option = document.createElement("option");
        option.innerText = part;
        option.value = part;
        partTypeSelection.appendChild(option);
    }
    
    removeButton.addEventListener(
        "click", 
        () => {
            console.log("remove button clicked");
            element.parentElement.removeChild(element);
        });
    
    createElementBtn.insertAdjacentElement("beforebegin", element);
}

async function createPostAsync() {
    const tagOptions = createPostSpace.querySelectorAll(".tag-entry");
    const tags = [];

    for (const tag of tagOptions){
        if (tag.selected) {
            tags.push(tag.value);
        }
    }

    const partWrappers = createPostSpace.querySelectorAll(".part-wrapper");
    const parts = [];

    for (const part of partWrappers) {
        const partTypeSelection = part.querySelector(".part-type-selection");
        const content = part.querySelector(".content-input");
        const link = part.querySelector("#link-input");

        parts.push(
            {
                "type": partTypeSelection.value,
                "content": content.value,
                "link": link.value
            }
        )
    }
    
    const relatedPostsCheckbox = createPostSpace.querySelectorAll(".related-post-select")
    const relatedPosts = [];
    
    for (const related of relatedPostsCheckbox) {
        if (related.checked) {
            relatedPosts.push(related.id);
        }
    }
    
    const payload = {
        "title": createPostSpace.querySelector("#post-title").value,
        "urlIdentifier": createPostSpace.querySelector("#post-identifier").value,
        "summary": createPostSpace.querySelector("#post-summary").value,
        "isPublished": createPostSpace.querySelector("#is-published-checkbox").checked,
        "tags": tags,
        "content": parts,
        "relatedPosts": relatedPosts
    }
    
    await fetch("/api/posts/", {
        method: "POST",
        body: JSON.stringify(payload),
        headers: {
            "Content-Type": "application/json"
        }
    })
}

document.addEventListener("DOMContentLoaded", () => {
    button.addEventListener("click", createPostAsync);
    createElementBtn.addEventListener("click", createElement);
})
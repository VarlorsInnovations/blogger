
const createPostSpace = document.getElementById("post-space");
const createElementBtn = document.getElementById("create-element-button");
const button = document.getElementById("create-post-button");

class ContentCreation extends HTMLElement {
    
    static template1 =
        `
            <button class="remove-button" type="button">Remove</button>
            <select class="part-type-selection">
                
            </select><br>
            <textarea  class="content-input" placeholder="Content">

            </textarea>
            <input type="text" class="link-input" disabled placeholder="link"/>
        `

    static template2 =
        `
            <button class="remove-button" type="button">Remove</button>
            <select class="part-type-selection">
                
            </select><br>
            <input type="file" class="content-input" placeholder="Content" alt="" />
            <button type="button" id="upload-button">Upload</button>
            
            <input type="text" class="link-input" disabled placeholder="link"/>
        `


    static partTypes = [
        "Heading1",
        "Heading2",
        "Heading3",
        "Heading4",
        "Paragraph",
        "Image",
        "Video",
        "Link"
    ]
    
    constructor() {
        super();
    }
    
    async connectedCallback() {
        this.transformIntoNormal("Heading1");
    }
    
    transformIntoNormal(value) {
        const template = new DOMParser().parseFromString(ContentCreation.template1, "text/html");
        this.innerHTML = template.body.innerHTML;

        this.guiContent = {
            partTypeSelect: this.querySelector(".part-type-selection"),
            removeButton: this.querySelector(".remove-button"),
        }
        
        this.#initSelection(value);
        
        this.guiContent.removeButton.addEventListener("click",
            () => this.parentElement.removeChild(this));

        this.guiContent.partTypeSelect.addEventListener("change", ()=> {
            const current = this.guiContent.partTypeSelect.value;
            if (current === "Image") {
                this.transformIntoImage();
            }
            else {
                this.transformIntoNormal(current)
            }
        });
    }
    
    #initSelection(value) {
        for (const part of ContentCreation.partTypes) {
            const option = document.createElement("option");
            option.innerText = part;
            option.value = part;
            this.guiContent.partTypeSelect.appendChild(option);
        }
        
        this.guiContent.partTypeSelect.value = value;
    }
    
    transformIntoImage() {
        this.innerHTML = "";
        const template = new DOMParser().parseFromString(ContentCreation.template2, "text/html");
        this.innerHTML = template.body.innerHTML;

        this.guiContent = {
            partTypeSelect: this.querySelector(".part-type-selection"),
            removeButton: this.querySelector(".remove-button"),
            imageSelection: this.querySelector(".content-input"),
            uploadButton: this.querySelector("#upload-button"),
            linkInput: this.querySelector(".link-input"),
        }
        
        this.#initSelection("Image");

        this.guiContent.removeButton.addEventListener("click",
            () => this.parentElement.removeChild(this));
        
        this.guiContent.partTypeSelect.addEventListener("change", ()=> {
            const current = this.guiContent.partTypeSelect.value;
            if (current === "Image") {
                this.transformIntoImage();
            }
            else {
                this.transformIntoNormal(current)
            }
        });
        
        this.guiContent.uploadButton.addEventListener("click", this.uploadAsync.bind(this));
    }

    async uploadAsync() {
        const data = new FormData();
        data.append("file", this.guiContent.imageSelection.files[0]);
        const response = await fetch("/api/PostPart/upload", {
            method: "POST",
            body: data,
            headers: { "Content-Type": "application/json" },
        })
        
        const json = await response.json();
        this.guiContent.linkInput.value = json["fileName"];
    }
}

customElements.define("content-creation-element", ContentCreation);

async function createElement() {
    const creationElement = new ContentCreation();
    createElementBtn.insertAdjacentElement("beforebegin", creationElement);
}

async function createPostAsync() {
    const tagOptions = createPostSpace.querySelectorAll(".tag-entry");
    const tags = [];

    for (const tag of tagOptions){
        if (tag.selected) {
            tags.push(tag.value);
        }
    }

    const partWrappers = createPostSpace.querySelectorAll("content-creation-element");
    const parts = [];

    for (const part of partWrappers) {
        const partTypeSelection = part.querySelector(".part-type-selection");
        const content = part.querySelector(".content-input");
        const link = part.querySelector(".link-input");

        parts.push(
            {
                "type": partTypeSelection.value,
                "content": content.value,
                "link": link.value
            }
        )
    }
    
    const partResponse = await fetch("/api/PostPart/", {
        method: "POST",
        body: JSON.stringify( {parts: parts}),
        headers: {
            "Content-Type": "application/json"
        }
    })
    
    if (!partResponse.ok) {
        throw new Error("Unable to create post parts");
    }
    
    const partJson = await partResponse.json();
    
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
        "content": partJson,
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
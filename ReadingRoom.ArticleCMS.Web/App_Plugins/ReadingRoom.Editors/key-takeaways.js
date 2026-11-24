class KeyTakeawaysElement extends HTMLElement {
    #items = [];
    #list;

    connectedCallback() {
        this.#list = document.createElement('div');

        const addButton = document.createElement('uui-button');
        addButton.label = 'Add takeaway';
        addButton.look = 'placeholder';
        addButton.addEventListener('click', () => {
            this.#items.push('');
            this.#showItems();
            this.#notifyChange();
        });

        this.appendChild(this.#list);
        this.appendChild(addButton);
        this.#showItems();
    }

    get value() {
        return [...this.#items];
    }

    set value(newValue) {
        const incoming = Array.isArray(newValue) ? newValue : [];

        let isSame = incoming.length === this.#items.length;
        for (let index = 0; isSame && index < incoming.length; index++) {
            if (incoming[index] !== this.#items[index]) {
                isSame = false;
            }
        }

        if (isSame) {
            return;
        }

        this.#items = [...incoming];
        if (this.#list) {
            this.#showItems();
        }
    }

    #showItems() {
        this.#list.replaceChildren();

        for (let index = 0; index < this.#items.length; index++) {
            const row = document.createElement('div');
            row.style.display = 'flex';
            row.style.gap = '8px';
            row.style.marginBottom = '8px';

            const input = document.createElement('uui-input');
            input.label = 'Takeaway ' + (index + 1);
            input.value = this.#items[index];
            input.style.flex = '1';
            input.addEventListener('input', () => {
                this.#items[index] = input.value;
                this.#notifyChange();
            });

            const removeButton = document.createElement('uui-button');
            removeButton.label = 'Remove takeaway ' + (index + 1);
            removeButton.textContent = 'Remove';
            removeButton.look = 'secondary';
            removeButton.color = 'danger';
            removeButton.addEventListener('click', () => {
                this.#items.splice(index, 1);
                this.#showItems();
                this.#notifyChange();
            });

            row.appendChild(input);
            row.appendChild(removeButton);
            this.#list.appendChild(row);
        }
    }

    #notifyChange() {
        this.dispatchEvent(new Event('change', { bubbles: true }));
    }
}

customElements.define('reading-room-key-takeaways', KeyTakeawaysElement);

export default KeyTakeawaysElement;
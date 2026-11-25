class KeyTakeawaysElement extends HTMLElement {
  #items = [];
  #list;
  #min = 0;
  #max = 0; // 0 means unlimited, the same rule as the built-in editor
  #placeholder = '';
  #addButton;
  #limitText;

  set config(config) {
    if (!config) {
      return;
    }

    this.#min = Number(config.getValueByAlias('min')) || 0;
    this.#max = Number(config.getValueByAlias('max')) || 0;
    this.#placeholder = config.getValueByAlias('placeholder') ?? '';

    if (this.#list) {
      this.#showItems();
    }
  }

  connectedCallback() {
    this.#list = document.createElement('div');

    this.#addButton = document.createElement('uui-button');
    this.#addButton.label = 'Add takeaway';
    this.#addButton.look = 'placeholder';
    this.#addButton.addEventListener('click', () => {
      this.#items.push('');
      this.#showItems();
      this.#notifyChange();
    });

    this.#limitText = document.createElement('span');
    this.#limitText.style.color = '#F97316';

    const footer = document.createElement('div');
    footer.style.display = 'flex';
    footer.style.alignItems = 'center';
    footer.style.gap = '12px';
    footer.appendChild(this.#addButton);
    footer.appendChild(this.#limitText);

    this.appendChild(this.#list);
    this.appendChild(footer);
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
      input.placeholder = this.#placeholder;
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
      removeButton.disabled = this.#items.length <= this.#min;
      removeButton.addEventListener('click', () => {
        this.#items.splice(index, 1);
        this.#showItems();
        this.#notifyChange();
      });

      row.appendChild(input);
      row.appendChild(removeButton);
      this.#list.appendChild(row);
    }
    this.#addButton.disabled = this.#max > 0 && this.#items.length >= this.#max;
    this.#limitText.textContent = this.#describeLimits();
  }

  #describeLimits() {
    if (this.#min > 0 && this.#max > 0) {
      return 'Takeaways: minimum ' + this.#min + ', maximum ' + this.#max;
    }

    if (this.#max > 0) {
      return 'Takeaways: maximum ' + this.#max;
    }

    if (this.#min > 0) {
      return 'Takeaways: minimum ' + this.#min;
    }

    return '';
  }

  #notifyChange() {
    this.dispatchEvent(new Event('change', { bubbles: true }));
  }
}

customElements.define('reading-room-key-takeaways', KeyTakeawaysElement);

export default KeyTakeawaysElement;

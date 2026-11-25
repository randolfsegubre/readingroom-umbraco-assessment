import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { html, css } from '@umbraco-cms/backoffice/external/lit';

export default class KeyTakeawaysElement extends UmbLitElement {
  static properties = {
    value: { attribute: false },
    _min: { state: true },
    _max: { state: true },
    _placeholder: { state: true },
  };

  constructor() {
    super();
    this.value = [];
    this._min = 0;
    this._max = 0;
    this._placeholder = '';
  }

  set config(config) {
    if (!config) {
      return;
    }

    this._min = Number(config.getValueByAlias('min')) || 0;
    this._max = Number(config.getValueByAlias('max')) || 0;
    this._placeholder = config.getValueByAlias('placeholder') ?? '';
  }

  #items() {
    return Array.isArray(this.value) ? this.value : [];
  }

  #setItems(items) {
    this.value = items;
    this.dispatchEvent(new Event('change', { bubbles: true }));
  }

  #addItem() {
    this.#setItems([...this.#items(), '']);
  }

  #changeItem(index, text) {
    const items = [...this.#items()];
    items[index] = text;
    this.#setItems(items);
  }

  #removeItem(index) {
    const items = [...this.#items()];
    items.splice(index, 1);
    this.#setItems(items);
  }

  #describeLimits() {
    if (this._min > 0 && this._max > 0) {
      return this.localize.term('readingRoom_limitsMinMax', this._min, this._max);
    }

    if (this._max > 0) {
      return this.localize.term('readingRoom_limitsMax', this._max);
    }

    if (this._min > 0) {
      return this.localize.term('readingRoom_limitsMin', this._min);
    }

    return '';
  }

  render() {
    const items = this.#items();

    return html`
      ${items.map(
      (item, index) => html`
          <div class="row">
            <uui-input
              label=${this.localize.term('readingRoom_takeawayLabel', index + 1)}
              placeholder=${this._placeholder}
              .value=${item}
              @input=${(event) => this.#changeItem(index, event.target.value)}></uui-input>
            <uui-button
              look="secondary"
              color="danger"
              label=${this.localize.term('readingRoom_removeTakeawayLabel', index + 1)}
              ?disabled=${items.length <= this._min}
              @click=${() => this.#removeItem(index)}>
              ${this.localize.term('readingRoom_remove')}
            </uui-button>
          </div>
        `
    )}
      <div class="footer">
        <uui-button
          look="placeholder"
          label=${this.localize.term('readingRoom_addTakeaway')}
          ?disabled=${this._max > 0 && items.length >= this._max}
          @click=${() => this.#addItem()}></uui-button>
        <span class="limits">${this.#describeLimits()}</span>
      </div>
    `;
  }

  static styles = css`
    .row {
      display: flex;
      gap: 8px;
      margin-bottom: 8px;
    }

    uui-input {
      flex: 1;
    }

    .footer {
      display: flex;
      align-items: center;
      gap: 12px;
    }

    .limits {
      color: var(--uui-color-warning-standalone);
    }
  `;
}

customElements.define('reading-room-key-takeaways', KeyTakeawaysElement);

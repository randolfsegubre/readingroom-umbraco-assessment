import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { html, css, repeat } from '@umbraco-cms/backoffice/external/lit';
import { UmbChangeEvent } from '@umbraco-cms/backoffice/event';
import { UmbFormControlMixin } from '@umbraco-cms/backoffice/validation';
import { UmbSorterController } from '@umbraco-cms/backoffice/sorter';

let editorCount = 0;

export default class KeyTakeawaysElement extends UmbFormControlMixin(UmbLitElement, undefined) {
  static properties = {
    _min: { state: true },
    _max: { state: true },
    _placeholder: { state: true },
  };

  // each row keeps its own id, so two rows with the same text can still be told apart while sorting.
  #entries = [];
  #lastId = 0;

  #sorter = new UmbSorterController(this, {
    getUniqueOfElement: (element) => element.getAttribute('data-entry-id'),
    getUniqueOfModel: (entry) => entry.id,
    identifier: `ReadingRoom.Sorter.KeyTakeaways.${++editorCount}`,
    itemSelector: '.row',
    containerSelector: '.rows',
    handleSelector: '.handle',
    onChange: ({ model }) => this.#setEntries(model),
  });

  constructor() {
    super();
    this._min = 0;
    this._max = 0;
    this._placeholder = '';

    // these stop Save and publish in the backoffice. Umbraco checks the same limits on the server.
    this.addValidator(
      'rangeUnderflow',
      () => this.#tooFewMessage(),
      () => this.#isTooFew()
    );
    this.addValidator(
      'rangeOverflow',
      () => this.#tooManyMessage(),
      () => this.#isTooMany()
    );
  }

  set config(config) {
    if (!config) {
      return;
    }

    this._min = Number(config.getValueByAlias('min')) || 0;
    this._max = Number(config.getValueByAlias('max')) || 0;
    this._placeholder = config.getValueByAlias('placeholder') ?? '';
  }

  willUpdate(changedProperties) {
    super.willUpdate(changedProperties);

    // rebuild the rows only when the value was changed from outside, so typing keeps the focus.
    const items = this.#items();
    const isSame = items.length === this.#entries.length
      && items.every((text, index) => text === this.#entries[index].text);

    if (!isSame) {
      this.#entries = items.map((text) => this.#newEntry(text));
      this.#sorter.setModel(this.#entries);
    }
  }

  #items() {
    return Array.isArray(this.value) ? this.value : [];
  }

  #newEntry(text) {
    this.#lastId += 1;
    return { id: String(this.#lastId), text };
  }

  #setEntries(entries) {
    this.#entries = entries;
    this.#sorter.setModel(entries);
    this.value = entries.map((entry) => entry.text);
    this.pristine = false;
    this.dispatchEvent(new UmbChangeEvent());
  }

  #addItem() {
    this.#setEntries([...this.#entries, this.#newEntry('')]);
  }

  #changeItem(index, text) {
    const entries = [...this.#entries];
    entries[index] = { ...entries[index], text };
    this.#setEntries(entries);
  }

  #removeItem(index) {
    const entries = [...this.#entries];
    entries.splice(index, 1);
    this.#setEntries(entries);
  }

  #moveItem(index, step) {
    const entries = [...this.#entries];
    const [entry] = entries.splice(index, 1);
    entries.splice(index + step, 0, entry);
    this.#setEntries(entries);
  }

  #isTooFew() {
    return this._min > 0 && this.#items().length < this._min;
  }

  #isTooMany() {
    return this._max > 0 && this.#items().length > this._max;
  }

  #tooFewMessage() {
    return this.localize.term('readingRoom_tooFew', this._min, this.#items().length);
  }

  #tooManyMessage() {
    return this.localize.term('readingRoom_tooMany', this._max, this.#items().length);
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

  #renderLimits() {
    // Umbraco shows the validation message under the editor, so only the colour changes here.
    const isOutside = this.#isTooFew() || this.#isTooMany();

    return html`<span class=${isOutside ? 'error' : 'limits'}>${this.#describeLimits()}</span>`;
  }

  #renderRow(entry, index) {
    const count = this.#entries.length;

    return html`
      <div class="row" data-entry-id=${entry.id}>
        <uui-icon name="icon-grip" class="handle"></uui-icon>
        <uui-input
          label=${this.localize.term('readingRoom_takeawayLabel', index + 1)}
          placeholder=${this._placeholder}
          .value=${entry.text}
          @input=${(event) => this.#changeItem(index, event.target.value)}
          @change=${(event) => event.stopPropagation()}></uui-input>
        <uui-button
          compact
          look="secondary"
          label=${this.localize.term('readingRoom_moveUpLabel', index + 1)}
          ?disabled=${index === 0}
          @click=${() => this.#moveItem(index, -1)}>
          <uui-icon name="icon-arrow-up"></uui-icon>
        </uui-button>
        <uui-button
          compact
          look="secondary"
          label=${this.localize.term('readingRoom_moveDownLabel', index + 1)}
          ?disabled=${index === count - 1}
          @click=${() => this.#moveItem(index, 1)}>
          <uui-icon name="icon-arrow-down"></uui-icon>
        </uui-button>
        <uui-button
          look="secondary"
          color="danger"
          label=${this.localize.term('readingRoom_removeTakeawayLabel', index + 1)}
          ?disabled=${count <= this._min}
          @click=${() => this.#removeItem(index)}>
          ${this.localize.term('readingRoom_remove')}
        </uui-button>
      </div>
    `;
  }

  render() {
    return html`
      <div class="rows">
        ${repeat(
          this.#entries,
          (entry) => entry.id,
          (entry, index) => this.#renderRow(entry, index)
        )}
      </div>
      <div class="footer">
        <uui-button
          look="placeholder"
          label=${this.localize.term('readingRoom_addTakeaway')}
          ?disabled=${this._max > 0 && this.#entries.length >= this._max}
          @click=${() => this.#addItem()}></uui-button>
        ${this.#renderLimits()}
      </div>
    `;
  }

  static styles = css`
    .row {
      display: flex;
      align-items: center;
      gap: 8px;
      margin-bottom: 8px;
    }

    .handle {
      cursor: grab;
    }

    .--umb-sorter-placeholder {
      opacity: 0.3;
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

    .error {
      color: var(--uui-color-danger-standalone);
      font-weight: 700;
    }
  `;
}

customElements.define('reading-room-key-takeaways', KeyTakeawaysElement);

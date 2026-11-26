import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { html } from '@umbraco-cms/backoffice/external/lit';
import { UmbChangeEvent } from '@umbraco-cms/backoffice/event';

// these values match the ones in the Core class ReadingLevels.
const LEVELS = [
  { value: 'beginner', term: 'readingRoom_levelBeginner' },
  { value: 'intermediate', term: 'readingRoom_levelIntermediate' },
  { value: 'advanced', term: 'readingRoom_levelAdvanced' },
];

export default class ReadingLevelElement extends UmbLitElement {
  static properties = {
    value: { attribute: false },
  };

  constructor() {
    super();
    this.value = '';
  }

  #select(event) {
    event.stopPropagation();
    this.value = event.target.value;
    this.dispatchEvent(new UmbChangeEvent());
  }

  #options() {
    const current = this.value ?? '';
    const options = [
      { value: '', name: this.localize.term('readingRoom_levelNotSet'), selected: current === '' },
    ];

    for (const level of LEVELS) {
      options.push({
        value: level.value,
        name: this.localize.term(level.term),
        selected: current === level.value,
      });
    }

    return options;
  }

  render() {
    return html`
      <uui-select
        label=${this.localize.term('readingRoom_readingLevel')}
        .options=${this.#options()}
        @change=${(event) => this.#select(event)}></uui-select>
    `;
  }
}

customElements.define('reading-room-reading-level', ReadingLevelElement);

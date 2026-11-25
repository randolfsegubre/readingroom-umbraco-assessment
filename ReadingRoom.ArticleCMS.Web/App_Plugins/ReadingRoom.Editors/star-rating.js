import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { html, css } from '@umbraco-cms/backoffice/external/lit';
import { UmbChangeEvent } from '@umbraco-cms/backoffice/event';

const DEFAULT_MAXIMUM = 5;

export default class StarRatingElement extends UmbLitElement {
  static properties = {
    value: { attribute: false },
    _max: { state: true },
  };

  constructor() {
    super();
    this.value = 0;
    this._max = DEFAULT_MAXIMUM;
  }

  set config(config) {
    if (!config) {
      return;
    }

    this._max = Number(config.getValueByAlias('max')) || DEFAULT_MAXIMUM;
  }

  #rating() {
    return Number(this.value) || 0;
  }

  #setRating(rating) {
    this.value = rating;
    this.dispatchEvent(new UmbChangeEvent());
  }

  #renderStar(star) {
    const rating = this.#rating();

    return html`
      <uui-button
        compact
        label=${this.localize.term('readingRoom_rateStars', star, this._max)}
        aria-pressed=${star === rating ? 'true' : 'false'}
        @click=${() => this.#setRating(star)}>
        <span class="star">${star <= rating ? '★' : '☆'}</span>
      </uui-button>
    `;
  }

  render() {
    const stars = Array.from({ length: this._max }, (_, index) => index + 1);
    const rating = this.#rating();

    return html`
      <div class="stars">
        ${stars.map((star) => this.#renderStar(star))}
        <uui-button
          look="secondary"
          label=${this.localize.term('readingRoom_clearRating')}
          ?disabled=${rating === 0}
          @click=${() => this.#setRating(0)}></uui-button>
      </div>
    `;
  }

  static styles = css`
    .stars {
      display: flex;
      align-items: center;
      gap: 4px;
    }

    .star {
      font-size: 24px;
      color: var(--uui-color-warning-standalone);
    }
  `;
}

customElements.define('reading-room-star-rating', StarRatingElement);

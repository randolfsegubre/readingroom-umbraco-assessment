import { UmbCollectionViewElementBase } from '@umbraco-cms/backoffice/collection';
import { UMB_APP_LANGUAGE_CONTEXT } from '@umbraco-cms/backoffice/language';
import { UMB_EDIT_DOCUMENT_WORKSPACE_PATH_PATTERN } from '@umbraco-cms/backoffice/document';
import { html, css, nothing, repeat } from '@umbraco-cms/backoffice/external/lit';

const LEVEL_TERMS = {
  beginner: 'readingRoom_levelBeginner',
  intermediate: 'readingRoom_levelIntermediate',
  advanced: 'readingRoom_levelAdvanced',
};

const STATE_TERMS = {
  Published: 'readingRoom_statePublished',
  PublishedPendingChanges: 'readingRoom_statePendingChanges',
};

const MAXIMUM_STARS = 5;

/**
 * A view of the Article List collection that shows each article as a card, with its publish date,
 * reading level, rating and publish state in the language chosen in the backoffice.
 */
export default class ArticleCardsViewElement extends UmbCollectionViewElementBase {
  static properties = {
    _culture: { state: true },
  };

  constructor() {
    super();
    this.consumeContext(UMB_APP_LANGUAGE_CONTEXT, (context) => {
      this.observe(
        context?.appLanguageCulture,
        (culture) => {
          this._culture = culture;
        },
        'observeCulture'
      );
    });
  }

  #variant(item) {
    return item.variants.find((variant) => variant.culture === this._culture) ?? item.variants[0];
  }

  #value(item, alias) {
    return item.values?.find(
      (entry) => entry.alias === alias && (!entry.culture || entry.culture === this._culture)
    )?.value;
  }

  #renderCard(item) {
    const variant = this.#variant(item);
    const publishDate = String(this.#value(item, 'publishDate') ?? '').slice(0, 10);
    const levelTerm = LEVEL_TERMS[this.#value(item, 'readingLevel')];
    const rating = Math.min(Math.max(Number(this.#value(item, 'editorRating')) || 0, 0), MAXIMUM_STARS);
    const stateTerm = STATE_TERMS[variant?.state] ?? 'readingRoom_stateDraft';

    return html`
      <a class="card" href=${UMB_EDIT_DOCUMENT_WORKSPACE_PATH_PATTERN.generateAbsolute({ unique: item.unique })}>
        <strong>${variant?.name}</strong>
        <span>${publishDate}</span>
        ${levelTerm ? html`<span>${this.localize.term(levelTerm)}</span>` : nothing}
        ${rating > 0
          ? html`<span class="stars" title=${this.localize.term('readingRoom_ratedStars', rating, MAXIMUM_STARS)}>
              ${'★'.repeat(rating)}${'☆'.repeat(MAXIMUM_STARS - rating)}
            </span>`
          : nothing}
        <uui-tag color=${stateTerm === 'readingRoom_statePublished' ? 'positive' : 'warning'}>
          ${this.localize.term(stateTerm)}
        </uui-tag>
      </a>
    `;
  }

  render() {
    return html`
      <div class="cards">
        ${repeat(
          this._items ?? [],
          (item) => item.unique,
          (item) => this.#renderCard(item)
        )}
      </div>
    `;
  }

  static styles = css`
    .cards {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(220px, 1fr));
      gap: 16px;
      padding: 12px 0;
    }

    .card {
      display: flex;
      flex-direction: column;
      align-items: flex-start;
      gap: 6px;
      padding: 16px;
      border: 1px solid var(--uui-color-border);
      border-radius: var(--uui-border-radius);
      background: var(--uui-color-surface);
      color: var(--uui-color-text);
      text-decoration: none;
    }

    .card:hover {
      border-color: var(--uui-color-interactive-emphasis);
    }

    .stars {
      color: var(--uui-color-warning-standalone);
    }
  `;
}

customElements.define('reading-room-article-cards-view', ArticleCardsViewElement);

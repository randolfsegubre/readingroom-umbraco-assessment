import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UMB_AUTH_CONTEXT } from '@umbraco-cms/backoffice/auth';
import { html, css } from '@umbraco-cms/backoffice/external/lit';

const STATS_PATH = '/umbraco/management/api/v1/reading-room/stats';

export default class ArticleStatsDashboardElement extends UmbLitElement {
  static properties = {
    _stats: { state: true },
    _errorMessage: { state: true },
  };

  #authContext;

  constructor() {
    super();
    this._stats = undefined;
    this._errorMessage = undefined;

    this.consumeContext(UMB_AUTH_CONTEXT, (authContext) => {
      this.#authContext = authContext;
      this.#loadStats();
    });
  }

  async #loadStats() {
    const config = this.#authContext.getOpenApiConfiguration();
    const token = await config.token();

    const response = await fetch(config.base + STATS_PATH, {
      credentials: config.credentials,
      headers: { Authorization: 'Bearer ' + token },
    });

    if (!response.ok) {
      this._errorMessage = 'The article statistics could not be loaded (status ' + response.status + ').';
      return;
    }

    this._stats = await response.json();
  }

  render() {
    if (this._errorMessage) {
      return html`<uui-box headline="Article stats"><p>${this._errorMessage}</p></uui-box>`;
    }

    if (!this._stats) {
      return html`<uui-box headline="Article stats"><uui-loader></uui-loader></uui-box>`;
    }

    return html`
            <uui-box headline="Article stats">
                <p>Published articles: <strong>${this._stats.totalArticles}</strong></p>
                <h3>Latest articles</h3>
                <ul>
                    ${this._stats.latestArticles.map(
      (article) => html`<li>${article.title} <small>(${article.publishDate.substring(0, 10)})</small></li>`
    )}
                </ul>
            </uui-box>
        `;
  }

  static styles = css`
        :host {
            display: block;
            padding: var(--uui-size-layout-1);
        }
    `;
}

customElements.define('reading-room-article-stats-dashboard', ArticleStatsDashboardElement);

import { Pipe, PipeTransform, inject } from '@angular/core';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { marked } from 'marked';

// GFM + treat single newlines as <br> so LLM responses with line breaks render naturally.
marked.use({ gfm: true, breaks: true });

@Pipe({ name: 'markdown', standalone: true })
export class MarkdownPipe implements PipeTransform {
  private readonly sanitizer = inject(DomSanitizer);

  // The LLM is treated as a trusted source for our personal-app threat model.
  // If we ever need to render user-authored markdown, swap to sanitizer.sanitize().
  transform(value: string | null | undefined): SafeHtml {
    if (!value) return '';
    const html = marked.parse(value, { async: false }) as string;
    return this.sanitizer.bypassSecurityTrustHtml(html);
  }
}

import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-navbar',
  imports: [RouterLink],
  template: `
    <nav>
      <a routerLink="/">Catalogue</a>
    </nav>
  `,
})
export class NavbarComponent {}

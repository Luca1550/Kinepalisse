import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-compte-layout',
  imports: [RouterLink, RouterLinkActive, RouterOutlet],
  templateUrl: './compte-layout.html',
  styleUrl: './compte-layout.css',
})
export class CompteLayoutComponent {}

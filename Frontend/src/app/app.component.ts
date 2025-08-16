import { Component } from "@angular/core";
import { CommonModule } from "@angular/common";
import { RouterOutlet } from "@angular/router";

@Component({
  selector: "app-root",
  standalone: true,
  imports: [CommonModule, RouterOutlet],
  template: `
    <div class="app-container">
      <header>
        <h1>{{ title }}</h1>
      </header>
      <main>
        <router-outlet></router-outlet>
      </main>
    </div>
  `,
  styles: [
    `
      .app-container {
        min-height: 100vh;
        display: flex;
        flex-direction: column;
      }
      header {
        background-color: #1976d2;
        color: white;
        padding: 1rem;
        text-align: center;
      }
      main {
        flex: 1;
        padding: 2rem;
      }
    `,
  ],
})
export class AppComponent {
  title = "AI-Powered Legal Strategy Advisor";
}

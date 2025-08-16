import { bootstrapApplication } from "@angular/platform-browser";
import { importProvidersFrom } from "@angular/core";
import { RouterModule } from "@angular/router";
import { HttpClientModule } from "@angular/common/http";
import { BrowserAnimationsModule } from "@angular/platform-browser/animations";

import { AppComponent } from "./app/app.component";
import { environment } from "./environments/environment";

if (environment.production) {
  // Enable production mode
}

bootstrapApplication(AppComponent, {
  providers: [
    importProvidersFrom(
      RouterModule.forRoot([]),
      HttpClientModule,
      BrowserAnimationsModule
    ),
  ],
}).catch((err) => console.error(err));

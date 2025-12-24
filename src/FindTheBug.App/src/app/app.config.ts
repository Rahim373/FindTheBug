import { ApplicationConfig, provideZoneChangeDetection, importProvidersFrom, APP_INITIALIZER } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideAnimations } from '@angular/platform-browser/animations';
import { en_US, provideNzI18n } from 'ng-zorro-antd/i18n';
import { registerLocaleData } from '@angular/common';
import en from '@angular/common/locales/en';
import { FormsModule } from '@angular/forms';
import { provideNzIcons, NzIconService } from 'ng-zorro-antd/icon';
import {
  UserOutline,
  LockOutline,
  MailOutline,
  EyeOutline,
  EyeInvisibleOutline,
  ArrowLeftOutline,
  DashboardOutline,
  SettingOutline,
  LogoutOutline,
  FilePdfOutline,
  ExperimentOutline,
  FileTextOutline,
  PlusOutline,
  InboxOutline,
  MenuOutline,
  DownOutline,
  MedicineBoxFill,
  TeamOutline,
  SecurityScanFill,
  HomeOutline,
  MedicineBoxOutline
} from '@ant-design/icons-angular/icons';

import { routes } from './app.routes';
import { authInterceptor } from './core/interceptors/auth.interceptor';

registerLocaleData(en);

// Font Awesome icon registration function
function registerFontAwesomeIcons(iconService: NzIconService): () => void {
  return () => {
    // Register Font Awesome icons as custom SVG icons
    // User Doctor icon
    iconService.addIconLiteral('fa:user-doctor', `
      <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 448 512">
        <path d="M224 256A128 128 0 1 0 224 0a128 128 0 1 0 0 256zm-96 55.2C54 332.9 0 401.3 0 482.3C0 498.7 13.3 512 29.7 512H418.3c16.4 0 29.7-13.3 29.7-29.7c0-81-54-149.4-128-171.1V362c27.6 7.1 48 32.2 48 62v40c0 8.8-7.2 16-16 16H336c-8.8 0-16-7.2-16-16s7.2-16 16-16V424c0-17.7-14.3-32-32-32s-32 14.3-32 32v24c8.8 0 16 7.2 16 16s-7.2 16-16 16H256c-8.8 0-16-7.2-16-16V424c0-29.8 20.4-54.9 48-62V304.9c-6-.6-12.1-.9-18.3-.9H178.3c-6.2 0-12.3 .3-18.3 .9v65.4c23.1 6.9 40 28.3 40 53.7c0 30.9-25.1 56-56 56s-56-25.1-56-56c0-25.4 16.9-46.8 40-53.7V311.2zM144 448a24 24 0 1 0 0-48 24 24 0 1 0 0 48z"/>
      </svg>
    `);

    // Hospital User icon
    iconService.addIconLiteral('fa:hospital-user', `
      <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 576 512">
        <path d="M48 0C21.5 0 0 21.5 0 48V256H144c8.8 0 16 7.2 16 16s-7.2 16-16 16H0v64H144c8.8 0 16 7.2 16 16s-7.2 16-16 16H0v80c0 26.5 21.5 48 48 48H265.9c-6.3-10.2-9.9-22.2-9.9-35.1c0-46.9 25.8-87.8 64-109.2V271.8 48c0-26.5-21.5-48-48-48H48zM152 64h16c8.8 0 16 7.2 16 16v24h24c8.8 0 16 7.2 16 16v16c0 8.8-7.2 16-16 16H184v24c0 8.8-7.2 16-16 16H152c-8.8 0-16-7.2-16-16V152H112c-8.8 0-16-7.2-16-16V120c0-8.8 7.2-16 16-16h24V80c0-8.8 7.2-16 16-16zM512 272a80 80 0 1 0 -160 0 80 80 0 1 0 160 0zM288 477.1c0 19.3 15.6 34.9 34.9 34.9H541.1c19.3 0 34.9-15.6 34.9-34.9c0-51.4-41.7-93.1-93.1-93.1H381.1c-51.4 0-93.1 41.7-93.1 93.1z"/>
      </svg>
    `);

    // Stethoscope icon
    iconService.addIconLiteral('fa:stethoscope', `
      <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 576 512">
        <path d="M142.4 21.9c5.6 16.8-3.5 34.9-20.2 40.5L96 71.1V192c0 53 43 96 96 96s96-43 96-96V71.1l-26.1-8.7c-16.8-5.6-25.8-23.7-20.2-40.5s23.7-25.8 40.5-20.2l26.1 8.7C334.4 19.1 352 43.5 352 71.1V192c0 77.2-54.6 141.6-127.3 156.7C231 404.6 278.4 448 336 448c61.9 0 112-50.1 112-112V265.3c-28.3-12.3-48-40.5-48-73.3c0-44.2 35.8-80 80-80s80 35.8 80 80c0 32.8-19.7 61-48 73.3V336c0 97.2-78.8 176-176 176c-92.9 0-168.9-71.9-175.5-163.1C87.2 334.2 32 269.6 32 192V71.1c0-27.5 17.6-52 43.8-60.7l26.1-8.7c16.8-5.6 34.9 3.5 40.5 20.2zM480 224a32 32 0 1 0 0-64 32 32 0 1 0 0 64z"/>
      </svg>
    `);

    // Syringe icon
    iconService.addIconLiteral('fa:syringe', `
      <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 512 512">
        <path d="M441 7l32 32 32 32c9.4 9.4 9.4 24.6 0 33.9s-24.6 9.4-33.9 0l-15-15L417.9 128l55 55c9.4 9.4 9.4 24.6 0 33.9s-24.6 9.4-33.9 0l-72-72L295 73c-9.4-9.4-9.4-24.6 0-33.9s24.6-9.4 33.9 0l55 55L422.1 56 407 41c-9.4-9.4-9.4-24.6 0-33.9s24.6-9.4 33.9 0zM210.3 155.7l61.1-61.1c.3 .3 .6 .7 1 1l16 16 56 56 56 56 16 16c.3 .3 .6 .6 1 1l-61.1 61.1c-.3-.3-.6-.6-1-1l-16-16-56-56-56-56-16-16c-.3-.3-.6-.6-1-1zm-77.9 61.1l20.1 20.1L68.9 320.4c-10.4 10.4-16.4 24.6-16.4 39.4V408c0 17.7-14.3 32-32 32s-32-14.3-32-32V359.8c0-32.1 12.7-62.9 35.3-85.5L107.4 190.6l25 25zm70.1-38.5l16 16 56 56 56 56 16 16-17.4 17.4c-10.1 10.1-23.6 15.7-37.8 15.7H272c-17.7 0-32 14.3-32 32s14.3 32 32 32h19.4c31.7 0 62.1-12.6 84.6-35l17.4-17.4c.3 .3 .6 .6 1 1l16 16c9.4 9.4 24.6 9.4 33.9 0s9.4-24.6 0-33.9l-16-16-56-56-56-56-16-16c-9.4-9.4-24.6-9.4-33.9 0s-9.4 24.6 0 33.9z"/>
      </svg>
    `);
  };
}

const icons = [
  UserOutline,
  LockOutline,
  MailOutline,
  EyeOutline,
  EyeInvisibleOutline,
  ArrowLeftOutline,
  DashboardOutline,
  SettingOutline,
  LogoutOutline,
  FilePdfOutline,
  ExperimentOutline,
  FileTextOutline,
  PlusOutline,
  InboxOutline,
  MenuOutline,
  DownOutline,
  MedicineBoxFill,
  TeamOutline,
  SecurityScanFill,
  HomeOutline,
  MedicineBoxOutline
];

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(withInterceptors([authInterceptor])),
    provideAnimations(),
    provideNzI18n(en_US),
    importProvidersFrom(FormsModule),
    provideNzIcons(icons),
    {
      provide: APP_INITIALIZER,
      useFactory: registerFontAwesomeIcons,
      deps: [NzIconService],
      multi: true
    }
  ]
};

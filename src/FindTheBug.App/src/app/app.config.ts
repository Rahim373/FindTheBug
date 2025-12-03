import { ApplicationConfig, provideZoneChangeDetection, importProvidersFrom } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideAnimations } from '@angular/platform-browser/animations';
import { en_US, provideNzI18n } from 'ng-zorro-antd/i18n';
import { registerLocaleData } from '@angular/common';
import en from '@angular/common/locales/en';
import { FormsModule } from '@angular/forms';
import { provideNzIcons } from 'ng-zorro-antd/icon';
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
    DownOutline
} from '@ant-design/icons-angular/icons';

import { routes } from './app.routes';
import { authInterceptor } from './core/interceptors/auth.interceptor';

registerLocaleData(en);

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
    DownOutline
];

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(withInterceptors([authInterceptor])),
    provideAnimations(),
    provideNzI18n(en_US),
    importProvidersFrom(FormsModule),
    provideNzIcons(icons)
  ]
};

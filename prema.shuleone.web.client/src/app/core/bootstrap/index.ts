export * from './menu.service';
export * from './settings.service';
export * from './startup.service';
export * from './preloader.service';
export * from './translate-lang.service';

import { APP_INITIALIZER } from '@angular/core';
import { TranslateLangService } from './translate-lang.service';
import { StartupService } from './startup.service';
import { environment } from '@env/environment';
import { KeycloakService } from 'keycloak-angular';

export function initializeApp(keycloak: KeycloakService, startupService: StartupService) {
  return () => new Promise<any>((resolve, reject) => {
    keycloak.init({
      config: {
        url: environment.keycloakUrl,
        realm: environment.realm,
        clientId: environment.clientId,
      },
      initOptions: {
        onLoad: 'login-required',
        checkLoginIframe: false,
      },
      loadUserProfileAtStartUp: true,
      bearerExcludedUrls: ['/assets'],
    })
    .then(() => {
      console.log('Keycloak initialized successfully');
      return startupService.load();
    })
    .then(() => {
      console.log('StartupService loaded successfully');
      resolve(true);
    })
    .catch((error) => {
      console.error('Error during initialization', error);
      reject(error);
    });
  });
}

export function TranslateLangServiceFactory(translateLangService: TranslateLangService) {
  return () => translateLangService.load();
}

export function StartupServiceFactory(startupService: StartupService) {
  return () => startupService.load();
}

export const appInitializerProviders = [
  {
    provide: APP_INITIALIZER,
    useFactory: initializeApp,
    multi: true,
    deps: [KeycloakService, StartupService]
  },
  {
    provide: APP_INITIALIZER,
    useFactory: TranslateLangServiceFactory,
    deps: [TranslateLangService],
    multi: true,
  },
  // {
  //   provide: APP_INITIALIZER,
  //   useFactory: StartupServiceFactory,
  //   deps: [StartupService],
  //   multi: true,
  // },
];

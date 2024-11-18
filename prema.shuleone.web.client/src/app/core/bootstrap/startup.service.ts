import { Injectable, inject } from '@angular/core';
import { AuthService, User } from '@core/authentication';
import { NgxPermissionsService, NgxRolesService } from 'ngx-permissions';
import { catchError, switchMap, tap } from 'rxjs';
import { Menu, MenuService } from './menu.service';
import { KeycloakService } from 'keycloak-angular';

@Injectable({
  providedIn: 'root',
})
export class StartupService {
  private readonly authService = inject(AuthService);
  private readonly menuService = inject(MenuService);
  private readonly permissionsService = inject(NgxPermissionsService);
  private readonly rolesService = inject(NgxRolesService);
  private readonly keycloakService = inject(KeycloakService);

  /**
   * Load the application only after get the menu or other essential informations
   * such as permissions and roles.
   */

  load() {
    return new Promise<void>((resolve, reject) => {
      this.authService
        .change()
        .pipe(
          tap(user => console.log("User received from change():", user)), // Debug log
          tap(user => this.setPermissions()),
          switchMap(() => this.authService.menu()),
          tap(menu => this.setMenu(menu))
        )
        .subscribe({
          next: () => resolve(),
          error: (err) => {
            console.error("Error during load:", err);
            resolve();
          },
        });
    });
  }
  
  
  private setMenu(menu: Menu[]) {
    this.menuService.addNamespace(menu, 'menu');
    this.menuService.set(menu);
  }

  private async setPermissions() {
    console.log("setting permisions")
    // In a real app, you should get permissions and roles from the user information.
    const roles = this.keycloakService.getUserRoles();
    const user = this.keycloakService.loadUserProfile();
    console.log("user: ",JSON.stringify(user))

    const permissions = ['canAdd', 'canDelete', 'canEdit', 'canRead'];
    this.permissionsService.loadPermissions(permissions);
    this.rolesService.flushRoles();
    // this.rolesService.addRoles({ roles: permissions });
    roles.forEach(role => {
      this.rolesService.addRoles({
        [role]: permissions
      });
    });
  }
}

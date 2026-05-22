import { Injectable, signal, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { tap } from 'rxjs';

export interface UserInfo { id: number; email: string; role: string; }

@Injectable({ providedIn: 'root' })
export class AuthService {
  private http = inject(HttpClient);
  private readonly KEY = 'kine_jwt';

  // État utilisateur sous forme de signal (lisible dans les templates)
  private _user = signal<UserInfo | null>(this.readFromStorage());
  readonly user = this._user.asReadonly();

  register(nom: string, prenom: string, email: string, motDePasse: string) {
    return this.http.post<{ idClient: number }>('/api/auth/register',
      { nom, prenom, email, motDePasse });
  }

  login(email: string, motDePasse: string) {
    return this.http.post<{ token: string }>('/api/auth/login', { email, motDePasse })
      .pipe(tap(res => {
        localStorage.setItem(this.KEY, res.token);
        this._user.set(this.decode(res.token));
      }));
  }

  logout(): void {
    localStorage.removeItem(this.KEY);
    this._user.set(null);
  }

  token(): string | null {
    return localStorage.getItem(this.KEY);
  }

  hasRole(role: string): boolean {
    return this._user()?.role === role;
  }

  // Décode le payload base64url du JWT — pas de vérification de signature ici,
  // c'est juste pour afficher l'email/rôle. La vérification reste côté serveur.
  private decode(token: string): UserInfo {
    const payload = token.split('.')[1];
    const json = atob(payload.replace(/-/g, '+').replace(/_/g, '/'));
    const data = JSON.parse(decodeURIComponent(escape(json)));
    return {
      id:    Number(data.sub),
      // ASP.NET sérialise parfois les claims avec des URLs longues → on essaie les deux noms
      email: data.email
        ?? data['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'],
      role:  data.role
        ?? data['http://schemas.microsoft.com/ws/2008/06/identity/claims/role']
    };
  }

  private readFromStorage(): UserInfo | null {
    const t = localStorage.getItem(this.KEY);
    return t ? this.decode(t) : null;
  }
}

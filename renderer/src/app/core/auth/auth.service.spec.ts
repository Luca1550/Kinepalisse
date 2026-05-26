import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { AuthService } from './auth.service';

// Faux JWT — payload : {"sub":"1","email":"test@kine.be","role":"Client"}
const FAKE_TOKEN =
  'eyJhbGciOiJIUzI1NiJ9' +
  '.eyJzdWIiOiIxIiwiZW1haWwiOiJ0ZXN0QGtpbmUuYmUiLCJyb2xlIjoiQ2xpZW50In0' +
  '.fakesig';

describe('AuthService', () => {
  let service: AuthService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    localStorage.clear();
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting(), AuthService],
    });
    service = TestBed.inject(AuthService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
    localStorage.clear();
  });

  it('login stocke le token dans localStorage', () => {
    service.login('test@kine.be', 'mdp123').subscribe();
    httpMock.expectOne('/api/auth/login').flush({ token: FAKE_TOKEN });
    expect(localStorage.getItem('kine_jwt')).toBe(FAKE_TOKEN);
  });

  it('logout retire le token du localStorage', () => {
    localStorage.setItem('kine_jwt', FAKE_TOKEN);
    service.logout();
    expect(localStorage.getItem('kine_jwt')).toBeNull();
  });

  it('hasRole retourne true pour le bon rôle et false pour un autre', () => {
    service.login('test@kine.be', 'mdp123').subscribe();
    httpMock.expectOne('/api/auth/login').flush({ token: FAKE_TOKEN });
    expect(service.hasRole('Client')).toBe(true);
    expect(service.hasRole('Admin')).toBe(false);
  });
});

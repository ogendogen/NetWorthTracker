import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { API_BASE_URL } from '../../app.config';
import { AuthService } from './auth.service';

describe('AuthService', () => {
  let service: AuthService;
  let httpTesting: HttpTestingController;

  beforeEach(() => {
    sessionStorage.clear();
    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        { provide: API_BASE_URL, useValue: 'https://api.example.test' },
      ],
    });

    service = TestBed.inject(AuthService);
    httpTesting = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpTesting.verify();
    sessionStorage.clear();
  });

  it('posts registration credentials without creating a session', () => {
    const credentials = {
      username: 'new-user',
      email: 'new-user@example.test',
      password: 'secret',
    };
    let result: { success: boolean } | undefined;

    service.register(credentials).subscribe((response) => (result = response));

    const request = httpTesting.expectOne('https://api.example.test/register');
    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual(credentials);
    request.flush({ success: true });

    expect(result).toEqual({ success: true });
    expect(service.session()).toBeNull();
    expect(service.isAuthenticated()).toBe(false);
    expect(sessionStorage.getItem('net-worth-tracker.session')).toBeNull();
  });

  it('posts a forgot-password request without creating a session', () => {
    const request = { email: 'user@example.test' };
    let result: { success: boolean } | undefined;

    service.forgotPassword(request).subscribe((response) => (result = response));

    const httpRequest = httpTesting.expectOne('https://api.example.test/forgot-password');
    expect(httpRequest.request.method).toBe('POST');
    expect(httpRequest.request.body).toEqual(request);
    httpRequest.flush({ success: true });

    expect(result).toEqual({ success: true });
    expect(service.session()).toBeNull();
    expect(sessionStorage.getItem('net-worth-tracker.session')).toBeNull();
  });

  it('posts a reset-password request without creating a session', () => {
    const request = { token: 'reset-token', password: 'Secret1!', confirmPassword: 'Secret1!' };
    let result: { success: boolean } | undefined;

    service.resetPassword(request).subscribe((response) => (result = response));

    const httpRequest = httpTesting.expectOne('https://api.example.test/reset-password');
    expect(httpRequest.request.method).toBe('POST');
    expect(httpRequest.request.body).toEqual(request);
    httpRequest.flush({ success: true });

    expect(result).toEqual({ success: true });
    expect(service.session()).toBeNull();
    expect(sessionStorage.getItem('net-worth-tracker.session')).toBeNull();
  });
});

import { TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { provideRouter, Router } from '@angular/router';
import { provideTranslateService, TranslateService } from '@ngx-translate/core';
import { firstValueFrom, Observable, of, throwError } from 'rxjs';
import { ForgotPasswordRequest, ForgotPasswordResponse } from '../../core/auth/auth.models';
import { AuthService } from '../../core/auth/auth.service';
import { ForgotPasswordPageComponent } from './forgot-password.page';

describe('ForgotPasswordPageComponent', () => {
  const authService = {
    forgotPassword: vi.fn<(request: ForgotPasswordRequest) => Observable<ForgotPasswordResponse>>(),
  };

  beforeEach(async () => {
    authService.forgotPassword.mockReset();
    await TestBed.configureTestingModule({
      imports: [ForgotPasswordPageComponent],
      providers: [
        provideRouter([]),
        provideTranslateService({ lang: 'en' }),
        { provide: AuthService, useValue: authService },
      ],
    }).compileComponents();

    const translateService = TestBed.inject(TranslateService);
    translateService.setTranslation('en', {
      common: { productName: 'Net Worth Tracker' },
      forgotPassword: {
        requestFailed: 'We could not process your request. Try again in a moment.',
        backToSignIn: 'Back to sign in',
      },
    });
    await firstValueFrom(translateService.use('en'));
  });

  it('requires a valid email and does not submit an invalid form', () => {
    const fixture = TestBed.createComponent(ForgotPasswordPageComponent);
    const component = fixture.componentInstance;

    component.submit();

    expect(component.form.invalid).toBe(true);
    expect(component.form.controls.email.touched).toBe(true);
    expect(authService.forgotPassword).not.toHaveBeenCalled();

    component.form.setValue({ email: 'not-an-email' });
    component.submit();

    expect(component.form.controls.email.hasError('email')).toBe(true);
    expect(authService.forgotPassword).not.toHaveBeenCalled();
  });

  it('submits the email and navigates to the sent page', () => {
    authService.forgotPassword.mockReturnValue(of({ success: true }));
    const fixture = TestBed.createComponent(ForgotPasswordPageComponent);
    const component = fixture.componentInstance;
    const router = TestBed.inject(Router);
    const navigateByUrl = vi.spyOn(router, 'navigateByUrl').mockResolvedValue(true);
    component.form.setValue({ email: 'user@example.test' });

    component.submit();

    expect(authService.forgotPassword).toHaveBeenCalledWith({ email: 'user@example.test' });
    expect(navigateByUrl).toHaveBeenCalledWith('/forgot-password-sent');
    expect(component.isSubmitting()).toBe(false);
  });

  it('shows a generic error when the request fails', () => {
    authService.forgotPassword.mockReturnValue(throwError(() => new Error('network')));
    const fixture = TestBed.createComponent(ForgotPasswordPageComponent);
    const component = fixture.componentInstance;
    component.form.setValue({ email: 'user@example.test' });

    component.submit();
    fixture.detectChanges();

    expect(component.requestFailed()).toBe(true);
    const error = fixture.debugElement.query(By.css('.request-error'))
      .nativeElement as HTMLParagraphElement;
    expect(error.getAttribute('role')).toBe('alert');
    expect(error.textContent).toContain('We could not process your request.');
  });

  it('links back to the login page', () => {
    const fixture = TestBed.createComponent(ForgotPasswordPageComponent);
    fixture.detectChanges();

    const link = fixture.debugElement.query(By.css('.auth-link a'))
      .nativeElement as HTMLAnchorElement;
    expect(link.textContent).toContain('Back to sign in');
    expect(link.getAttribute('href')).toBe('/login');
  });
});

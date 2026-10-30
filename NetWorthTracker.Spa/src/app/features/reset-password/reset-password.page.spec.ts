import { TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { ActivatedRoute, convertToParamMap, provideRouter, Router } from '@angular/router';
import { provideTranslateService, TranslateService } from '@ngx-translate/core';
import { firstValueFrom, Observable, of, throwError } from 'rxjs';
import { ResetPasswordRequest, ResetPasswordResponse } from '../../core/auth/auth.models';
import { AuthService } from '../../core/auth/auth.service';
import { ResetPasswordPageComponent } from './reset-password.page';

describe('ResetPasswordPageComponent', () => {
  const authService = {
    resetPassword: vi.fn<(request: ResetPasswordRequest) => Observable<ResetPasswordResponse>>(),
  };

  async function setup(queryParams: Record<string, string>): Promise<void> {
    authService.resetPassword.mockReset();
    await TestBed.configureTestingModule({
      imports: [ResetPasswordPageComponent],
      providers: [
        provideRouter([]),
        provideTranslateService({ lang: 'en' }),
        {
          provide: ActivatedRoute,
          useValue: { snapshot: { queryParamMap: convertToParamMap(queryParams) } },
        },
        { provide: AuthService, useValue: authService },
      ],
    }).compileComponents();

    const translateService = TestBed.inject(TranslateService);
    translateService.setTranslation('en', {
      common: { productName: 'Net Worth Tracker' },
      resetPassword: {
        resetFailed: 'We could not reset your password. The link may be invalid or expired.',
        missingToken: 'This password reset link is incomplete. Request a new link to continue.',
        requestNewLink: 'Request a new link',
      },
    });
    await firstValueFrom(translateService.use('en'));
  }

  it('hides the form and offers a new link when the token is missing', async () => {
    await setup({});
    const fixture = TestBed.createComponent(ResetPasswordPageComponent);
    fixture.detectChanges();

    expect(fixture.componentInstance.hasToken).toBe(false);
    expect(fixture.debugElement.query(By.css('form'))).toBeNull();
    const message = fixture.debugElement.query(By.css('.missing-token'))
      .nativeElement as HTMLParagraphElement;
    expect(message.getAttribute('role')).toBe('alert');
    expect(message.textContent).toContain('This password reset link is incomplete.');
    const link = fixture.debugElement.query(By.css('a[mat-flat-button]'))
      .nativeElement as HTMLAnchorElement;
    expect(link.getAttribute('href')).toBe('/forgot-password');

    fixture.componentInstance.submit();
    expect(authService.resetPassword).not.toHaveBeenCalled();
  });

  it('requires both passwords and does not submit an invalid form', async () => {
    await setup({ token: 'reset-token' });
    const fixture = TestBed.createComponent(ResetPasswordPageComponent);
    const component = fixture.componentInstance;

    component.submit();

    expect(component.form.invalid).toBe(true);
    expect(component.form.controls.password.touched).toBe(true);
    expect(component.form.controls.confirmPassword.touched).toBe(true);
    expect(authService.resetPassword).not.toHaveBeenCalled();
  });

  it('rejects passwords that do not match', async () => {
    await setup({ token: 'reset-token' });
    const fixture = TestBed.createComponent(ResetPasswordPageComponent);
    const component = fixture.componentInstance;
    component.form.setValue({ password: 'Secret1!', confirmPassword: 'Different1!' });

    component.submit();

    expect(component.form.hasError('passwordMismatch')).toBe(true);
    expect(component.passwordMismatchVisible).toBe(true);
    expect(authService.resetPassword).not.toHaveBeenCalled();
  });

  it('submits the token with both passwords and navigates to the success page', async () => {
    await setup({ token: 'reset-token' });
    authService.resetPassword.mockReturnValue(of({ success: true }));
    const fixture = TestBed.createComponent(ResetPasswordPageComponent);
    const component = fixture.componentInstance;
    const router = TestBed.inject(Router);
    const navigateByUrl = vi.spyOn(router, 'navigateByUrl').mockResolvedValue(true);
    component.form.setValue({ password: 'Secret1!', confirmPassword: 'Secret1!' });

    component.submit();

    expect(authService.resetPassword).toHaveBeenCalledWith({
      token: 'reset-token',
      password: 'Secret1!',
      confirmPassword: 'Secret1!',
    });
    expect(navigateByUrl).toHaveBeenCalledWith('/password-reset-success');
    expect(component.isSubmitting()).toBe(false);
  });

  it('shows an error with a link to request a new token when the reset fails', async () => {
    await setup({ token: 'expired-token' });
    authService.resetPassword.mockReturnValue(throwError(() => new Error('400')));
    const fixture = TestBed.createComponent(ResetPasswordPageComponent);
    const component = fixture.componentInstance;
    component.form.setValue({ password: 'Secret1!', confirmPassword: 'Secret1!' });

    component.submit();
    fixture.detectChanges();

    expect(component.resetFailed()).toBe(true);
    const error = fixture.debugElement.query(By.css('.reset-error'))
      .nativeElement as HTMLParagraphElement;
    expect(error.getAttribute('role')).toBe('alert');
    expect(error.textContent).toContain('We could not reset your password.');
    const link = error.querySelector('a');
    expect(link?.getAttribute('href')).toBe('/forgot-password');
  });
});

import { TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { provideTranslateService, TranslateService } from '@ngx-translate/core';
import { provideRouter, Router } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { PasswordResetSuccessPageComponent } from './password-reset-success.page';

describe('PasswordResetSuccessPageComponent', () => {
  beforeEach(async () => {
    vi.useFakeTimers();
    await TestBed.configureTestingModule({
      imports: [PasswordResetSuccessPageComponent],
      providers: [provideRouter([]), provideTranslateService({ lang: 'en' })],
    }).compileComponents();

    const translateService = TestBed.inject(TranslateService);
    translateService.setTranslation('en', {
      common: { productName: 'Net Worth Tracker' },
      passwordResetSuccess: {
        title: 'Password reset',
        success: 'Your password has been changed. You can now sign in with your new password.',
        redirect: 'Automatic sign-in redirect in {{ seconds }}s.',
        signIn: 'Sign in now',
      },
    });
    await firstValueFrom(translateService.use('en'));
  });

  afterEach(() => {
    vi.useRealTimers();
  });

  it('shows the success message and an immediate sign-in link', () => {
    const fixture = TestBed.createComponent(PasswordResetSuccessPageComponent);
    fixture.detectChanges();

    const status = fixture.debugElement.query(By.css('.success-message'))
      .nativeElement as HTMLParagraphElement;
    const link = fixture.debugElement.query(By.css('a')).nativeElement as HTMLAnchorElement;

    expect(status.getAttribute('role')).toBe('status');
    expect(status.textContent).toContain('Your password has been changed.');
    expect(link.textContent).toContain('Sign in now');
    expect(link.getAttribute('href')).toBe('/login');
  });

  it('counts down and redirects to login after ten seconds', () => {
    const router = TestBed.inject(Router);
    const navigateSpy = vi.spyOn(router, 'navigateByUrl').mockResolvedValue(true);
    const fixture = TestBed.createComponent(PasswordResetSuccessPageComponent);
    fixture.detectChanges();

    expect(fixture.componentInstance.secondsRemaining()).toBe(10);

    vi.advanceTimersByTime(9999);
    expect(navigateSpy).not.toHaveBeenCalled();

    vi.advanceTimersByTime(1);
    expect(fixture.componentInstance.secondsRemaining()).toBe(0);
    expect(navigateSpy).toHaveBeenCalledOnce();
    expect(navigateSpy).toHaveBeenCalledWith('/login');
  });

  it('cancels the redirect when the page is destroyed', () => {
    const router = TestBed.inject(Router);
    const navigateSpy = vi.spyOn(router, 'navigateByUrl').mockResolvedValue(true);
    const fixture = TestBed.createComponent(PasswordResetSuccessPageComponent);

    fixture.detectChanges();
    fixture.destroy();
    vi.advanceTimersByTime(10000);

    expect(navigateSpy).not.toHaveBeenCalled();
  });
});

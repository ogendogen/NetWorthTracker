import { TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { provideRouter, Router } from '@angular/router';
import { provideTranslateService, TranslateService } from '@ngx-translate/core';
import { firstValueFrom } from 'rxjs';
import { EmailConfirmationFailedPageComponent } from './email-confirmation-failed.page';

describe('EmailConfirmationFailedPageComponent', () => {
  beforeEach(async () => {
    vi.useFakeTimers();
    await TestBed.configureTestingModule({
      imports: [EmailConfirmationFailedPageComponent],
      providers: [provideRouter([]), provideTranslateService({ lang: 'en' })],
    }).compileComponents();

    const translateService = TestBed.inject(TranslateService);
    translateService.setTranslation('en', {
      common: { productName: 'Net Worth Tracker' },
      emailConfirmationFailed: {
        title: 'Email confirmation failed',
        message: 'The confirmation link may be invalid or expired.',
        signIn: 'Sign in',
      },
    });
    await firstValueFrom(translateService.use('en'));
  });

  afterEach(() => {
    vi.useRealTimers();
  });

  it('shows the failure message and a sign-in link', () => {
    const fixture = TestBed.createComponent(EmailConfirmationFailedPageComponent);
    fixture.detectChanges();

    const status = fixture.debugElement.query(By.css('.confirmation-failed-message'))
      .nativeElement as HTMLParagraphElement;
    const link = fixture.debugElement.query(By.css('a')).nativeElement as HTMLAnchorElement;

    expect(status.getAttribute('role')).toBe('status');
    expect(status.textContent).toContain('The confirmation link may be invalid or expired.');
    expect(link.textContent).toContain('Sign in');
    expect(link.getAttribute('href')).toBe('/login');
  });

  it('does not automatically redirect to login', () => {
    const router = TestBed.inject(Router);
    const navigateSpy = vi.spyOn(router, 'navigateByUrl').mockResolvedValue(true);
    const fixture = TestBed.createComponent(EmailConfirmationFailedPageComponent);

    fixture.detectChanges();
    vi.advanceTimersByTime(10000);

    expect(navigateSpy).not.toHaveBeenCalled();
  });
});

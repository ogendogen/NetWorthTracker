import { TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { provideTranslateService, TranslateService } from '@ngx-translate/core';
import { provideRouter, Router } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { EmailConfirmedPageComponent } from './email-confirmed.page';

describe('EmailConfirmedPageComponent', () => {
  beforeEach(async () => {
    vi.useFakeTimers();
    await TestBed.configureTestingModule({
      imports: [EmailConfirmedPageComponent],
      providers: [provideRouter([]), provideTranslateService({ lang: 'en' })],
    }).compileComponents();

    const translateService = TestBed.inject(TranslateService);
    translateService.setTranslation('en', {
      common: { productName: 'Net Worth Tracker' },
      emailConfirmation: {
        title: 'Email confirmed',
        success: 'Your email address has been confirmed successfully.',
        redirect: 'Automatic sign-in redirect in {{ seconds }}s.',
        signIn: 'Sign in now',
      },
    });
    await firstValueFrom(translateService.use('en'));
  });

  afterEach(() => {
    vi.useRealTimers();
  });

  it('shows the confirmation and an immediate sign-in link', () => {
    const fixture = TestBed.createComponent(EmailConfirmedPageComponent);
    fixture.detectChanges();

    const status = fixture.debugElement.query(By.css('.confirmation-message'))
      .nativeElement as HTMLParagraphElement;
    const link = fixture.debugElement.query(By.css('a')).nativeElement as HTMLAnchorElement;

    expect(status.getAttribute('role')).toBe('status');
    expect(status.textContent).toContain('Your email address has been confirmed successfully.');
    expect(link.textContent).toContain('Sign in now');
    expect(link.getAttribute('href')).toBe('/login');
  });

  it('counts down and redirects to login after ten seconds', () => {
    const router = TestBed.inject(Router);
    const navigateSpy = vi.spyOn(router, 'navigateByUrl').mockResolvedValue(true);
    const fixture = TestBed.createComponent(EmailConfirmedPageComponent);
    fixture.detectChanges();

    expect(fixture.componentInstance.secondsRemaining()).toBe(10);

    vi.advanceTimersByTime(1000);
    expect(fixture.componentInstance.secondsRemaining()).toBe(9);

    vi.advanceTimersByTime(8999);
    expect(navigateSpy).not.toHaveBeenCalled();

    vi.advanceTimersByTime(1);
    expect(fixture.componentInstance.secondsRemaining()).toBe(0);
    expect(navigateSpy).toHaveBeenCalledOnce();
    expect(navigateSpy).toHaveBeenCalledWith('/login');
  });

  it('cancels the redirect when the page is destroyed', () => {
    const router = TestBed.inject(Router);
    const navigateSpy = vi.spyOn(router, 'navigateByUrl').mockResolvedValue(true);
    const fixture = TestBed.createComponent(EmailConfirmedPageComponent);

    fixture.detectChanges();
    fixture.destroy();
    vi.advanceTimersByTime(10000);

    expect(navigateSpy).not.toHaveBeenCalled();
  });
});

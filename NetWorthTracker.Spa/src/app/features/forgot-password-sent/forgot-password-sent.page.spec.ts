import { TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { provideRouter } from '@angular/router';
import { provideTranslateService, TranslateService } from '@ngx-translate/core';
import { firstValueFrom } from 'rxjs';
import { ForgotPasswordSentPageComponent } from './forgot-password-sent.page';

describe('ForgotPasswordSentPageComponent', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ForgotPasswordSentPageComponent],
      providers: [provideRouter([]), provideTranslateService({ lang: 'en' })],
    }).compileComponents();

    const translateService = TestBed.inject(TranslateService);
    translateService.setTranslation('en', {
      common: { productName: 'Net Worth Tracker' },
      forgotPasswordSent: {
        title: 'Check your email',
        message: 'If an account exists for that email address, we have sent a password reset link.',
        expiry: 'The reset link is valid for 1 hour.',
        signIn: 'Go to sign in',
      },
    });
    await firstValueFrom(translateService.use('en'));
  });

  it('shows the non-revealing confirmation and a sign-in link', () => {
    const fixture = TestBed.createComponent(ForgotPasswordSentPageComponent);
    fixture.detectChanges();

    const status = fixture.debugElement.query(By.css('.forgot-password-sent-message'))
      .nativeElement as HTMLParagraphElement;
    const link = fixture.debugElement.query(By.css('a')).nativeElement as HTMLAnchorElement;

    expect(status.getAttribute('role')).toBe('status');
    expect(status.textContent).toContain('If an account exists for that email address');
    expect(fixture.nativeElement.textContent).toContain('valid for 1 hour');
    expect(link.textContent).toContain('Go to sign in');
    expect(link.getAttribute('href')).toBe('/login');
  });
});

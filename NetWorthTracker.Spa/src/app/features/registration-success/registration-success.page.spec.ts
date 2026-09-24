import { TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { provideRouter } from '@angular/router';
import { provideTranslateService, TranslateService } from '@ngx-translate/core';
import { firstValueFrom } from 'rxjs';
import { RegistrationSuccessPageComponent } from './registration-success.page';

describe('RegistrationSuccessPageComponent', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RegistrationSuccessPageComponent],
      providers: [provideRouter([]), provideTranslateService({ lang: 'en' })],
    }).compileComponents();

    const translateService = TestBed.inject(TranslateService);
    translateService.setTranslation('en', {
      common: { productName: 'Net Worth Tracker' },
      registrationSuccess: {
        title: 'Check your email',
        emailSent: 'A confirmation email has been sent to your email address.',
        instruction: 'Click the link in the email to confirm your address.',
        expiry: 'The confirmation link is valid for 24 hours.',
        signIn: 'Go to sign in',
      },
    });
    await firstValueFrom(translateService.use('en'));
  });

  it('explains the confirmation email and its 24-hour validity', () => {
    const fixture = TestBed.createComponent(RegistrationSuccessPageComponent);
    fixture.detectChanges();

    const status = fixture.debugElement.query(By.css('[role="status"]'))
      .nativeElement as HTMLParagraphElement;
    const pageText = fixture.nativeElement.textContent as string;

    expect(status.textContent).toContain('A confirmation email has been sent');
    expect(pageText).toContain('Click the link in the email to confirm your address.');
    expect(pageText).toContain('The confirmation link is valid for 24 hours.');
  });

  it('provides a static link to login', () => {
    const fixture = TestBed.createComponent(RegistrationSuccessPageComponent);
    fixture.detectChanges();

    const link = fixture.debugElement.query(By.css('a')).nativeElement as HTMLAnchorElement;

    expect(link.textContent).toContain('Go to sign in');
    expect(link.getAttribute('href')).toBe('/login');
  });
});

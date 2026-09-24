import { TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { ActivatedRoute, convertToParamMap, provideRouter } from '@angular/router';
import { provideTranslateService, TranslateService } from '@ngx-translate/core';
import { BehaviorSubject } from 'rxjs';
import { firstValueFrom } from 'rxjs';
import { AuthService } from '../../core/auth/auth.service';
import { LoginPageComponent } from './login.page';

describe('LoginPageComponent', () => {
  const queryParamMap = new BehaviorSubject(convertToParamMap({}));

  beforeEach(async () => {
    queryParamMap.next(convertToParamMap({}));
    await TestBed.configureTestingModule({
      imports: [LoginPageComponent],
      providers: [
        provideRouter([]),
        provideTranslateService({ lang: 'en' }),
        {
          provide: ActivatedRoute,
          useValue: {
            queryParamMap: queryParamMap.asObservable(),
            snapshot: { queryParamMap: convertToParamMap({}) },
          },
        },
        { provide: AuthService, useValue: { login: vi.fn() } },
      ],
    }).compileComponents();

    const translateService = TestBed.inject(TranslateService);
    translateService.setTranslation('en', {
      common: { productName: 'Net Worth Tracker' },
      login: {
        subtitle: 'Sign in to view your financial overview',
        newUser: 'New to Net Worth Tracker?',
        createAccount: 'Create account',
      },
    });
    await firstValueFrom(translateService.use('en'));
  });

  it('links the startup login form to registration', () => {
    const fixture = TestBed.createComponent(LoginPageComponent);
    fixture.detectChanges();

    const link = fixture.debugElement.query(By.css('.auth-link a'))
      .nativeElement as HTMLAnchorElement;
    expect(link.textContent).toContain('Create account');
    expect(link.getAttribute('href')).toBe('/register');
  });

  it('does not show a registration success message on login', () => {
    const fixture = TestBed.createComponent(LoginPageComponent);
    fixture.detectChanges();
    expect(fixture.debugElement.query(By.css('.login-success'))).toBeNull();

    queryParamMap.next(convertToParamMap({ registered: 'true' }));
    fixture.detectChanges();

    expect(fixture.debugElement.query(By.css('.login-success'))).toBeNull();
  });

  it('marks empty fields as invalid after submitting', () => {
    const fixture = TestBed.createComponent(LoginPageComponent);
    const component = fixture.componentInstance;
    component.submit();

    expect(component.form.controls.username.touched).toBe(true);
    expect(component.form.controls.password.touched).toBe(true);
    expect(component.form.controls.username.invalid).toBe(true);
    expect(component.form.controls.password.invalid).toBe(true);
  });
});

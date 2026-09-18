import { TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { ActivatedRoute, convertToParamMap, provideRouter } from '@angular/router';
import { BehaviorSubject } from 'rxjs';
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
  });

  it('links the startup login form to registration', () => {
    const fixture = TestBed.createComponent(LoginPageComponent);
    fixture.detectChanges();

    const link = fixture.debugElement.query(By.css('.auth-link a'))
      .nativeElement as HTMLAnchorElement;
    expect(link.textContent).toContain('Create account');
    expect(link.getAttribute('href')).toBe('/register');
  });

  it('shows registration success only when requested by the query parameter', () => {
    const fixture = TestBed.createComponent(LoginPageComponent);
    fixture.detectChanges();
    expect(fixture.debugElement.query(By.css('.login-success'))).toBeNull();

    queryParamMap.next(convertToParamMap({ registered: 'true' }));
    fixture.detectChanges();

    const status = fixture.debugElement.query(By.css('.login-success'))
      .nativeElement as HTMLParagraphElement;
    expect(status.getAttribute('role')).toBe('status');
    expect(status.textContent).toContain('Account created. Sign in to continue.');
  });

  it('marks empty fields as invalid after submitting', () => {
    const fixture = TestBed.createComponent(LoginPageComponent);
    fixture.detectChanges();

    const component = fixture.componentInstance;
    component.submit();
    fixture.detectChanges();

    const fields = fixture.debugElement.queryAll(By.css('mat-form-field'));
    expect(component.form.controls.username.touched).toBe(true);
    expect(component.form.controls.password.touched).toBe(true);
    expect(
      fields.every((field) => field.nativeElement.classList.contains('mat-mdc-form-field-invalid')),
    ).toBe(true);
  });
});

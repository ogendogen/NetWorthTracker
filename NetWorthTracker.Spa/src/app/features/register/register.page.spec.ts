import { TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { provideRouter, Router } from '@angular/router';
import { Observable, of, Subject, throwError } from 'rxjs';
import { AuthService } from '../../core/auth/auth.service';
import { RegisterRequest, RegisterResponse } from '../../core/auth/auth.models';
import { RegisterPageComponent } from './register.page';

describe('RegisterPageComponent', () => {
  const authService = {
    register: vi.fn<(credentials: RegisterRequest) => Observable<RegisterResponse>>(),
  };

  beforeEach(async () => {
    authService.register.mockReset();
    await TestBed.configureTestingModule({
      imports: [RegisterPageComponent],
      providers: [provideRouter([]), { provide: AuthService, useValue: authService }],
    }).compileComponents();
  });

  it('requires every field and does not submit an invalid form', () => {
    const fixture = TestBed.createComponent(RegisterPageComponent);
    const component = fixture.componentInstance;

    component.submit();

    expect(component.form.invalid).toBe(true);
    expect(component.form.controls.username.touched).toBe(true);
    expect(component.form.controls.email.touched).toBe(true);
    expect(component.form.controls.password.touched).toBe(true);
    expect(component.form.controls.confirmPassword.touched).toBe(true);
    expect(authService.register).not.toHaveBeenCalled();
  });

  it('rejects passwords that do not match', () => {
    const fixture = TestBed.createComponent(RegisterPageComponent);
    const component = fixture.componentInstance;
    component.form.setValue({
      username: 'new-user',
      email: 'new-user@example.test',
      password: 'secret',
      confirmPassword: 'different',
    });

    component.submit();

    expect(component.form.hasError('passwordMismatch')).toBe(true);
    expect(authService.register).not.toHaveBeenCalled();
  });

  it('submits only API fields and redirects to login on success', () => {
    authService.register.mockReturnValue(of({ success: true }));
    const fixture = TestBed.createComponent(RegisterPageComponent);
    const component = fixture.componentInstance;
    const router = TestBed.inject(Router);
    const navigate = vi.spyOn(router, 'navigate').mockResolvedValue(true);
    component.form.setValue({
      username: 'new-user',
      email: 'new-user@example.test',
      password: 'secret',
      confirmPassword: 'secret',
    });

    component.submit();

    expect(authService.register).toHaveBeenCalledWith({
      username: 'new-user',
      email: 'new-user@example.test',
      password: 'secret',
    });
    expect(navigate).toHaveBeenCalledWith(['/login'], { queryParams: { registered: true } });
  });

  it('keeps the submit action disabled while registration is pending', () => {
    const response = new Subject<RegisterResponse>();
    authService.register.mockReturnValue(response);
    const fixture = TestBed.createComponent(RegisterPageComponent);
    const component = fixture.componentInstance;
    component.form.setValue({
      username: 'new-user',
      email: 'new-user@example.test',
      password: 'secret',
      confirmPassword: 'secret',
    });

    component.submit();
    fixture.detectChanges();

    const button = fixture.debugElement.query(By.css('button')).nativeElement as HTMLButtonElement;
    expect(component.isSubmitting()).toBe(true);
    expect(button.disabled).toBe(true);

    response.complete();
    expect(component.isSubmitting()).toBe(false);
  });

  it('shows an accessible failure message when registration fails', () => {
    authService.register.mockReturnValue(throwError(() => new Error('Registration failed')));
    const fixture = TestBed.createComponent(RegisterPageComponent);
    const component = fixture.componentInstance;
    component.form.setValue({
      username: 'new-user',
      email: 'new-user@example.test',
      password: 'secret',
      confirmPassword: 'secret',
    });

    component.submit();
    fixture.detectChanges();

    const error = fixture.debugElement.query(By.css('.registration-error'))
      .nativeElement as HTMLParagraphElement;
    expect(error.getAttribute('role')).toBe('alert');
    expect(error.textContent).toContain('We could not create your account.');
  });
});

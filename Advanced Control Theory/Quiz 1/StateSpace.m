clear all;
close all;
syms y(t) u(t) t s;
f1=diff(diff(diff(y(t),t),t),t)+7*diff(diff(y(t),t),t)+14*diff(y(t),t)+8*y(t);
f2=diff(diff(diff(u(t),t),t),t)+diff(diff(u(t),t),t)-2*diff(u(t),t)-5*u(t);
L1(t)=laplace(f1,t,s);
L2(t)=laplace(f2,t,s);

L3=(s^3+7*s^2-2*s-5)/(s*(s^3+7*s^2+14*s+8))%+((6*s+16)/(s^3+7*s^2+14*s+8));
ilaplace(L3,s,t)


a=[0 1 0;0 0 1;-8 -14 -7];
b=[0;0;1];
c=[-13 -16 -6];
d=[1];
sys=ss(a,b,c,d);
%initial(sys,x0);
E=eig(a);
y_0=[1;0;0];
u_0=[1;0;0];
Q=[1 1 1;E(1) E(2) E(3);E(1)^2 E(2)^2 E(3)^2];
A_bar=round((inv(Q))*a*Q);
B_bar=round(inv(Q)*b);
C_bar=round(c*Q);
D_bar=round(d);

x_0=((y_0-[D_bar 0 0;C_bar*B_bar D_bar 0;C_bar*A_bar*B_bar C_bar*B_bar D_bar]*u_0)\[C_bar;C_bar*A_bar;C_bar*A_bar^2])';
syms t;
y1=C_bar*exp(A_bar.*t)*x_0+C_bar*int(exp(A_bar.*(t-0))*B_bar,t)+1;
y2=((-5/8)+exp(-t)-(5/4)*exp(-2*t)+(15/8)*exp(-4*t))+(10/3)*exp(-t)-2*exp(-2*t)-(4/3)*exp(-4*t);

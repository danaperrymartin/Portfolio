%Linear System Representation for Helicopter

close all;
clear all;

Simulation_Time=5;
Simulation_step=0.1;

A = [0 1 0 0; 0 976 0 0; 0 0 0 1; 0 0 0 0];
B = [0 0; 0 12.6480816; 0 0; -33.345933008 0.506871743];
C = [0 488 0 0;0 0 0 0];
D = [6.3240 0;16.6729 -0.021648];
%x0=[20;-10;10;-100];

 sys = ss(A, B, C, D);
% [csys,T]=canon(sys,'modal',inf)
[num,den]=ss2tf(A, B, C, D, 1);
[num,den]=ss2tf(A, B, C, D, 2);
% bode(sys);
% nyquist(sys);
%initial(sys,x0)

%Linear-quadratic regulator Design using built in MATLAB functions
 Q=[0.5 0 0 0; 0 0.5 0 0;0 0 0.5 0;0 0 0 0.5];
 R=[0.5 0;0 0.5];
 N=[0.25 0.25;0.25 0.25;0.25 0.25;0.25 0.25];
% 
 [K, S, E] = lqr(sys,Q, R, N)

%[A,B,C,D]=linmod('LinSys')
%eval('LinSys')
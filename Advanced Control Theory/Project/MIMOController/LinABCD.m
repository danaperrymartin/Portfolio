%Linear System Representation for Helicopter

close all;
clear all;

Simulation_Time=200;
Simulation_step=1;

%% Linearized A,B,C,D matrices
A = [0 1 0 0; 0 24.4 0 0; 0 0 0 1; 0 0 0 0];
B = [0 0;12648.1 0; 0 0;-2011.69 -.444031];
C = [1 0 0 0;0 0 1 0];
D = [0 0;0 0];
%x0=[20;-10;10;-100];



sys = ss(A, B, C, D);
%initial(sys,x0)
%% 

%% Augmented A,B,C,D matrices for statefeedback with output integrator

A_bar=[A zeros(4,2);-C zeros(2,2)];
B_bar=[B;D];
C_bar=[C zeros(2,1)];

K_bar=place(A_bar,B_bar,[-10,-40,-10,-20,-30,-50]);
eig(A_bar-B_bar*K_bar);
K_bar1=[K_bar(:,1:4)];
K_bar2=[-K_bar(:,5:6)];

L=place(A',C',[-.1;-.4;-.1;-.5])';
%%

%% 
%Linear-quadratic regulator Design using built in MATLAB functions 
Q=[1 0 0 0;0 1 0 0;0 0 1 0;0 0 0 1];
R=[1 1;0 1];
%N=[0.25 0.25;0.25 0.25;0.25 0.25;0.25 0.25]; 
[K, S, E] = lqr(sys,Q, R);
%  %%  Calculation of Observer Matrices
%  Ac=[(A-B*K)];
%  pole1=-13615;
%  pole2=-33.4;
%  pole3=-.1;
%  pole4=-3.2;
%  
% %  L=place(A',C',[pole1;pole2;pole3;pole4]);  
% %  Ae=[A-L*C];
%% 
 
 
sim('MIMOController_statespace')
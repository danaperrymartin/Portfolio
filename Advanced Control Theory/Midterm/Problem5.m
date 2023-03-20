Tmax=20;
Dt=0.1;

A=[-1 1 0 0; 0 -1 0 0; 0 0 2 0; 0 0 0 -3];
B=[0 1; 1 0; 0 0; 1 0];
C=[0 1 0 1; 1 0 0 0];
D=[0 0;0 0];
x0=[1 1 1 1]
Qd=[1 1 1 1;-1 -1 2 -3;1 1 4 9; -1 -1 8 -27];
A_bar=(A\Qd)*Qd;
B_bar=B\Qd;
C_bar=C*Qd;

sys=ss(A,B,C,D);
%poles(sys);
%tf(sys)
%initial(sys,x0)
eval('Problem5_Sim')

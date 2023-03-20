%
%PLANT
%
A=[0 1 0 0; 0 0 1 0; 0 0 0 1; 1 2 3 4]
eig(A)
B=[0;0;0;1]
C=[1 0 1 0];
%
%STATE FEEDBACK - places poles at -1;delta(s)=(s+1)^4=s^4+4*s^3+6*s^2+4*s+1
%
F=[-2 -6 -9 -8]
Acl=A+B*F
eig(Acl)
%
%FULL-ORDER OBSERVER - places observer poles at s+6)^4=s^4+24*s^3+216*s^2+864*s+1296
%
Lt=acker(A',C',[6;6;6;6])
L=Lt'
Ao=A-L*C
Bo=[B L]
Co=eye(4)
%
% CHECK CLOSED-LOOP
%
Aclo=[A B*F;L*C Ao+B*F]
eig(Aclo)


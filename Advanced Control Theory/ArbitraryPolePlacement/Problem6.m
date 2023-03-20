clear all



b=[1 2 4];
a=[1 -2 -1 3];
Gs=tf(b,a);
[A, B, C, D]=tf2ss(b,a);
sys=ss(A, B, C, D);


d=[1271/283 634/283 -947/283];
c=[1 710/283 1357/283];
Cs=tf(d,c);

tfcl=(b*d)/(a*c+b*d);


[csys,T]=canon(sys,'companion')

G=tf(b,a);

ac=[0 0 -3;1 0 1; 0 1 2]

delta=[5; 10; 10; 5; 1];
Au=[1 0 1 0 0;-2 1 2 1 0; -1 -2 4 2 1; 3 -1 0 4 2; 0 3 0 0 4];
d=[-2; -1; 3; 0; 0];
coef=Au\(delta-d);



CM=[0 1 1 -1 1 1 1 -1;1 0 -1 0 1 0 -1 0;0 0 0 0 0 0 0 0;1 0 -3 0 9 0 -27 0];
rank(CM);

cm1=[0 -1 0 0 0;0 0 0 0 1; 0 0 -3 0 0;0 0 0 2 1];
rank(cm1);

cm2=[0 -1 0 0 1;0 0 0 0 0;0 0 -3 0 0;0 0 0 2 0];
rank(cm2);

cm31=[3 -1 0 0 0;0 3 0 0 1;0 0 0 0 0;0 0 0 5 1];
rank(cm31);

cm32=[3 -1 0 0 1; 0 3 0 0 0;0 0 0 0 0;0 0 0 5 0];
rank(cm32);

OM=[0 1 0 1;1 0 0 0;0 -1 0 -3;-1 1 0 0;0 1 0 9;1 1 0 0;0 -1 0 -27;-1 1 0 0]
rank(OM);

om11=[0 -1 0 0;0 0 0 0;0 0 -3 0;0 0 0 2;0 1 0 1];
rank(om11);

om12=[0 -1 0 0;0 0 0 0;0 0 -3 0;0 0 0 2;1 0 0 0];
rank(om12);

om31=[3 -1 0 0; 0 3 0 0;0 0 0 0;0 0 0 5;0 1 0 1];
rank(om31);

om32=[3 -1 0 0;0 3 0 0;0 0 0 0;0 0 0 5;1 0 0 0];
rank(om32);
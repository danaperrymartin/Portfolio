close all;
clear all;



gravity=9.81;                                               %gravitational constant in m/s^2
rho_air=1.22;                                               %density of air in kg/m^3

Simulation_DT=0.00125;
Simulation_Tmax=5;

Helicopter_N=3;                                             %Number of blades on the main rotor
Helicopter_mRotor_Omega=10;                                 %Angular Velocity of main rotor in m/s
Helicopter_mRotor_Radius=1;                                 %Radius of main rotor in m
Helicopter_tRotor_Radius=0.5;                               %Radius of tail rotor in m
Helicopter_mRotor_Area=pi*Helicopter_mRotor_Radius^2;       %Area of main rotor in m^2
Helicopter_mRotor_Ib=10;                                    %Rotational inertia of main rotor blade
Helicopter_tRotor_Area=pi*Helicopter_tRotor_Radius^2;       %Area of tail rotor in m^2
Helicopter_tDistance=3;                                     %Distance from tail rotor to center of gravity of fuselage in m
Helicopter_Fuselage_Mass=5;                                 %Mass of fuselage in kg
Helicopter_Fuselage_DragCoefficient=15;                     %Drag Coefficient of fuselage
Helicopter_Fuselage_Area=2;                                 %Area of fuselage in m^2
Helicopter_Fuselage_RotInertia=50;                          %Rotational Inertia of Fuselage

Disturbance_mass=0;                                         %Disturbance of in mass of helicopter in kg

%Theta_0=0.5;
%Omega_Tail_0=10;


sim('Helicopter_EqPts_Sim')

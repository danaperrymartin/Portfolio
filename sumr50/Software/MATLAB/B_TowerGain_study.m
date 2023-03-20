%% B_BL_PitchGain

clear all;
close all;

A1_Initialize;
tic;

Parameters.Turbine.String = 'SUMR-50_d2ct2s5';

%% Simulation Parameters
Name_Model                = 'AC_TwrDmp';        
Name_Control              = 'C_BL_SUMR50';
Name_FAedit               = 'A3_OFedit_master';                          % Fast/Aerodyn Edit Script

FA.TMax      = 100;
FA.DT        = 0.0125;  % The timestep at which OpenFAST will simulate

AD.SkewMod   = 1;       % Used with ADv15; set to 1 if skew angle of inflow > 45 deg
%% Disturbance Setup
    IW.WindType             = 2;        % 1=steady; 2=uniform; 3=binary TurbSim FF; 4=binary Bladed-style FF; 5=HAWC format; 6=User defined
    Disturbance.Type        = 5;
    IW.HWindSpeed           = 11;
    InflowWindRoot          = [basePath,'\Analysis\WindFiles\SUMR-D_Tests\const\'];  % Wind file directory                            % Wind file directory
    IW.Filename             = 'sstep.wnd';              
    Disturbance.Vshear      = 0; 
    Disturbance.LVshear     = 0;
    Disturbance.LHshear     = 0;
    Disturbance.U_ref       = IW.HWindSpeed;
    Disturbance.Step        = 2;
    Disturbance.Tgust       = 15;
    Disturbance.Vgust       = 2;
    Disturbance.Tstart      = 50;
    Disturbance.Tend        = 50;                                       % Wind files are ~700s
    Disturbance.freq        = 0.4640;   %[0,0.4640,1.4640,2.4640]
    Disturbance.amp         = 2;
    
 %% Save Information
    PP.Save.Dir = ['../../Analysis/',Parameters.Turbine.String,'/TowerDamper/Step/'];
    PP.Save.Name = 'placeholder';
    PP.Save.Enable = 0;

%% Specify Turbine Parameter Values
if strcmp(Parameters.Turbine.String,'SUMR-50_d2ct2s5')
omega_r = rpm2radps(4.19);
Mt = 7559051;
Ta = -12360000;
Kt = omega_r^2*Mt;
Ct = -(-Kt-Mt);
    elseif strcmp(Parameters.Turbine.String,'SUMR-D')
    omega_r = 5.67;
    Mt = 36572;
    Ta = -808240;
    Ct = 129830;
    Kt = omega_r^2*Mt;
end
%% Natural Frequency and Damping Ratio
zeta   = [0.1];

for izeta = 1:length(zeta)
    disp(['Simulating turbine with ',num2str(IW.HWindSpeed), ' m/s wind and ', num2str(zeta(izeta)),' Tower Gain']);
    
    %Set tower FA gain
    PitchControlParams.Kqdot_0 = (2*zeta(izeta)*omega_r+(Ct/Mt))*-(Mt/Ta);  % Gain on tower fore-aft velocity
    PitchControlParams.Kq_0    = 0;%(2*zeta(izeta)*omega_r+(Ct/Mt))*-(Mt/Ta); % Gain on integral of tower fore-aft vel
    
    %Set Wind File & Save Name
     PP.Save.Name     =[Parameters.Turbine.String,'_','const_',num2str(IW.HWindSpeed),'_',num2str(zeta(izeta))];
     PP.Save.Enable   = 1;
    
    %Run Simulation
    A2_1_Sim_OpenFAST;
    disp(['Saving results to ',fullfile(PP.Save.Dir,PP.Save.Name)]);   
end

toc;
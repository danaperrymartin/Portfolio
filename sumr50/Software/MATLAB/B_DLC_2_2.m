%% Run DLC 2.2 - Extreme Turbulence Model with Pitch fault

% clear;
A1_Initialize; tic;

% Controller/Turbine Setup
DLC          = 'ETM';
% Parameters.Turbine.String = 'SUMR-50_v3';
% Name_Model   = 'BL_CollectivePitch_Shutdown';
% Name_Control = 'C_BL_SUMR50';
Name_FAedit  = 'A3_OFedit_master';  %Fast/Aerodyn Edit Script

% FAST Input File Modifications
FA.TMax         = 700;          % Wind Files are 700s
FA.DT           = 0.0125;
FA.CompElast    = 1;

AD.SkewMod = 1;

% Disturbance Setup
IW.WindType       = 3;        % 1=steady; 2=uniform; 3=binary TurbSim FF; 4=binary Bladed-style FF; 5=HAWC format; 6=User defined
InflowWindRoot    = 'D:\SUMR_Data\Wind_Files\SUMR-50_Tests\DLC_1_3_50MW_ClassI\';  % Wind file directory
Disturbance.File  = 'placeholder';
MeanWindSpeeds    = [10,12,24];
WindClass         = {'1ETM_B'};
Number            = [1];
% Save Information
PP.Save.Enable = 1;
PP.Save.Control = 'baseline';
PP.Save.DLC = 'DLC2.2';
if ~exist('AD','var')
    PP.Save.AD_ver = 'AD_v15';
elseif isfield(AD,'CompAero')
    if AD.CompAero == 1
        PP.Save.AD_ver = 'AD_v14';
    else
        PP.Save.AD_ver = 'AD_v15';
    end
else
    PP.Save.AD_ver = 'AD_v15';
end
PP.Save.Teeter = 'No_Teeter';
PP.Save.Name = 'placeholder';

pitchrate  = 8;
brake      = 9.163;
timewindow = 0.15;
PitchFaultTimeArray = [350];
% PitchFaultTime = 250;
GridLossTimeArray = [350];
GridLossTime = 500;
PitchRampRate1 = 12;
PitchRampRate2 = 2;
PitchRunawayRate = 18;
FaultDelay = 1;
GenTorqueStopRate = 800/1e3; % Rate at which Gen Torque approaches zero during stop (kn-M)

stuckBlade = 0;
runawayBlade = 0;

for iMeanWindSpeed = 1:length(MeanWindSpeeds)
    for iWindClass = 1:length(WindClass)
        for iNumber = Number
            for iFaultTime = 1:length(PitchFaultTimeArray)
            for iGridLossTime = 1:length(GridLossTimeArray)
            
            PitchFaultTime     = PitchFaultTimeArray(iFaultTime);
            GridLossTime       = GridLossTimeArray(iGridLossTime);
            InitiateTime_pitch = FA.TMax/2;
            InitiateTime_brake = FA.TMax/2;
            rate               = pitchrate;
            HSSBrakeTorq       = brake;
            BrakeTime          = timewindow;                                   
            
            % Set Wind File & Save Name
            Disturbance.File = [WindClass{iWindClass},'_',num2str(MeanWindSpeeds(iMeanWindSpeed)),'_',num2str(iNumber)];
            PP.Save.Name = [Parameters.Turbine.String,'_',WindClass{iWindClass},'_',num2str(MeanWindSpeeds(iMeanWindSpeed)),'_',num2str(iNumber),'_NoTeeter_Shutdown'];
             
            % Save Information
            PP.Save.WindCase = [WindClass{iWindClass},'_',num2str(MeanWindSpeeds(iMeanWindSpeed)),'_',num2str(iNumber)];
            IW.Filename = [InflowWindRoot,PP.Save.WindCase,'.bts'];
             PP.Save.Dir = ['..',filesep,'..',filesep,'Analysis',filesep,...
                Parameters.Turbine.String,filesep,PP.Save.Control,filesep,...
                PP.Save.DLC,filesep,PP.Save.AD_ver,filesep,PP.Save.Teeter];
            PP.Save.Enable = 1;
    
            disp(['Simulating Turbine w/ ',Disturbance.File,'.bts']);
            
            % Run FAST
            A2_1_Sim_OpenFAST;
            end
        end
    end
    end
end
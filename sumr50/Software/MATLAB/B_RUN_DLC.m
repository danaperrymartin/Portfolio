 %% Run Set of DLC's for a Turbine Configuration
% Dana Martin
% 12.15.18

clear all
close all

Turbine = {'SUMR-50_v6_s9'};
       
Name_Model           = 'BL_CollectivePitch';        
Name_Control         = 'C_BL_SUMR50';
Name_Model_Shutdown  = 'BL_CollectivePitch_Shutdown';
       
for ii = 1:length(Turbine)
    Parameters.Turbine.String = Turbine{ii};
B_const_wind;
% B_DLC_1_1;
% B_DLC_1_3;
% B_DLC_1_4;
% B_DLC_1_5;
% B_DLC_2_2;
% B_DLC_2_3;
% B_DLC_3_2;
% B_DLC_5_1;
% B_DLC_6_1;
end
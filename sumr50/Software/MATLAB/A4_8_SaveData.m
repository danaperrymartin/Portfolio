%% A4_8_SaveData
% Inputs: PP.SaveDir
%         PP.SaveName
%
% Will save timeseries data and .mat file with more goodies

if ~isdir(PP.Save.Dir)
    mkdir(PP.Save.Dir);
end

save(fullfile(PP.Save.Dir,[PP.Save.Name,'.mat']),...
    'Simulation','Parameters','Disturbance','PP','Chan','OutData','OutList',...
    'TorqueControlParams','PitchControlParams','OLControlParams');
movefile([PP.Save.Dir,'\FASTinput.SFunc.out'],fullfile(PP.Save.Dir,[PP.Save.Name,'.out']));
% rename(PP.Save.Dir,'FASTinput.SFunc.out',[PP.Save.Name,'.out']);
% copyfile(PP.Save.Dir,'FASTinput.SFunc.out',[PP.Save.Name,'.out']);

% copyfile('./FAST8_IF/FASTinput.fst',fullfile(PP.Save.Dir,'FASTinput.fst'));
% copyfile('./FAST8_IF/InflowWind.ipt',fullfile(PP.Save.Dir,'InflowWind.ipt'));
% copyfile('./FAST8_IF/AeroDyn.ipt',fullfile(PP.Save.Dir,'AeroDyn.ipt'));
% copyfile('./FAST8_IF/ElastoDyn.dat',fullfile(PP.Save.Dir,[PP.Save.Name,'ElastoDyn.dat']));
% copyfile('./FAST8_IF/ServoDyn.dat',fullfile(PP.Save.Dir,[PP.Save.Name,'ServoDyn.dat']));
% copyfile('./FAST8_IF/FASTinput.SFunc.AD.sum',fullfile(PP.Save.Dir,'FASTinput.SFunc.AD.sum'));
% copyfile('./FAST8_IF/FASTinput.SFunc.ED.sum',fullfile(PP.Save.Dir,'FASTinput.SFunc.ED.sum'));
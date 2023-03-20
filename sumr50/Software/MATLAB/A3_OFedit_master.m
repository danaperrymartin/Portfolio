%% A3_OFedit_master
% This file will edit the values of the OpenFAST input files that you
% define in the B_RUN_# files that are different from the default values.
%% Get Template Files
TemplateDir = '.\FAST8_IF\Templates';
Defs = Af_getInputTemplates(Parameters.Turbine.String);

Name_FAST_Def   = fullfile(TemplateDir,Defs.FAST);
Name_ED_Def     = fullfile(TemplateDir,Defs.ED);
% Name_AD_Def     = fullfile(TemplateDir,Defs.AD14);
Name_IW_Def     = fullfile(TemplateDir,Defs.IW);
Name_SD_Def     = fullfile(TemplateDir,Defs.SD);
Name_AD15_Def   = fullfile(TemplateDir,Defs.AD15);
Name_AD15_Blade = Defs.AD15_Blade;
Name_BD_Def     = fullfile(TemplateDir,Defs.BD);
%%
Af_EditAero15BasePath(Name_AD15_Def,basePath);
% Af_EditAero14BasePath(Name_AD14_Def,basePath);
Af_EditElastBasePath(Name_ED_Def,basePath);

if ~isdir(PP.Save.Dir)
    mkdir(PP.Save.Dir);
end

%% New input file names
Name_FAST_New   = fullfile(PP.Save.Dir,'FASTinput');
Name_AD_New     = fullfile(PP.Save.Dir,'AeroDyn');
Name_ED_New     = fullfile(PP.Save.Dir,'ElastoDyn');
Name_IW_New     = fullfile(PP.Save.Dir,'InflowWind');
Name_BD_New     = fullfile(PP.Save.Dir,'BeamDyn');
Name_SD_New     = fullfile(PP.Save.Dir,'ServoDyn');

%% Copy necessary files to the Save Directory
% Make this more robust to different file names in the future
files = dir(PP.Save.Dir);
filenames={files.name};
if ~exist(fullfile([PP.Save.Dir,'\AeroDyn.ipt']))
copyfile(['./FAST8_IF/',Defs.Twr,'.dat'],fullfile(PP.Save.Dir));
copyfile(['./FAST8_IF/',Defs.ED_Blade],fullfile(PP.Save.Dir));
copyfile(['./FAST8_IF/',Defs.AD15_Blade],fullfile(PP.Save.Dir));
copyfile(['./FAST8_IF/Aero15Data'],fullfile(PP.Save.Dir));
copyfile(['./FAST8_IF/',Defs.BD_Blade],fullfile(PP.Save.Dir));
end
fast_input_variable_names;

if ~exist('FA','var')
    FA.CompAero = 2;
    FA.CompElast = 1;
    FA.CompSerco = 1;
    FA.CompInflow = 1;
    default_FAST = 1;
end

%% AeroDyn
% AeroDynList = who('AD_*');

if ~isfield(FA,'CompAero')
    FA.CompAero = 2;
    default_FAST = 1;
end

if ~exist('AD','var')
    AD = struct();
    default_aero = 1;
end

AeroDynList = fieldnames(AD);

if FA.CompAero == 2
    % Use AeroDyn_v15 as default
    if ~isempty(AeroDynList)
        % If there are user-defined AD_input values, modify the master
        % AeroDyn.ipt file.
        for i=1:1:length(AeroDynList)
            var_check = ~isempty(find(strcmp(AeroEditNames,AeroDynList{i}),1));
            if var_check == 0
                error(['Invalid AD_input variable, "',AeroDynList{i},'" does not exist. Check your spelling',...
                    ' or if that variable exists in the current OpenFAST version.'])
            end
        end
        
        % Create the appropriate cell arrays for the Af_EditAero function.
%         AeroEditLine = strrep(AeroDynList,'AD_','');
        AeroEditLine = AeroDynList;
        AeroEditValue = cell(length(AeroEditLine),1);
        for i=1:1:length(AeroEditLine)
            AeroEditValue{i} = eval(['AD.',AeroDynList{i}]);
        end
        
        Af_EditAero15(AeroEditLine,AeroEditValue,Name_AD_New,Name_AD15_Def);
    else
        % If there are no edits, then the Master input file will be copied.
        AeroEditLine = {};
        AeroEditValue = {};
        Af_EditAero15(AeroEditLine,AeroEditValue,Name_AD_New,Name_AD15_Def);
    end
elseif FA.CompAero == 1
    % If the simulation is set to use AeroDyn_v14 (else default to AD_v15)
    if ~isempty(AeroDynList)
        % If there are user-defined AD.input values, modify the master
        % AeroDyn.ipt file.
        for i=1:1:length(AeroDynList)
            var_check = ~isempty(find(strcmp(AeroEditNames,AeroDynList{i}),1));
            if var_check == 0
                error(['Invalid AD.input variable, "',AeroDynList{i},'" does not exist. Check your spelling',...
                    ' or if that variable exists in the current OpenFAST version.'])
            end
        end
        
        % Create the appropriate cell arrays for the Af_EditAero function.
%         AeroEditLine = strrep(AeroDynList,'AD_','');
        AeroEditLine = AeroDynList;
        AeroEditValue = cell(length(AeroEditLine),1);
        for i=1:1:length(AeroEditLine)
            AeroEditValue{i} = eval(['AD.',AeroDynList{i}]);
        end
        
        Af_EditAero(AeroEditLine,AeroEditValue,Name_AD_New,Name_AD14_Def);
    else
        % If there are no edits, then the Master input file will be copied.
        AeroEditLine = {};
        AeroEditValue = {};
        Af_EditAero(AeroEditLine,AeroEditValue,Name_AD_New,Name_AD14_Def);
    end
elseif FA.CompAero == 0
    % If AeroDyn is not enabled.
    disp('AeroDyn not enabled in FAST.fst file.');
else
    error('Not a valid option for FA.CompAero.');
end

clear AeroDynList AeroEditLine AeroEditValue i var_check
if exist('default_aero','var')
    clear default_aero FA.CompAero
end
%% ElastoDyn
% ElastoDynList = who('ED_*');
ED.GBRatio      = Parameters.Turbine.G;
ED.PreCone_1    = Parameters.Turbine.ConeAngle;
ED.PreCone_2    = Parameters.Turbine.ConeAngle;
ED.PreCone_3    = Parameters.Turbine.ConeAngle;
ED.RotSpeed     = Parameters.Turbine.IC.Wr;
% ED.DT           = FA.DT;
if ~isfield(FA,'CompElast')
    % Default to just ElastoDyn (not ElastoDyn + BeamDyn)
    FA.CompElast = 1;
    default_FAST = 1;
end

if ~exist('ED')
    ED = struct();
    default_elast = 1;
end

ElastoDynList = fieldnames(ED);

% if FA.CompElast == 2
%     disp('ElastoDyn + BeamDyn not enabled yet with this MATLAB script');
if FA.CompElast == 1 || FA.CompElast == 2
    if ~isempty(ElastoDynList)
        % If there are user-defined ED.input values, modify the master ElastoDyn.ipt file.
        for i=1:1:length(ElastoDynList)
            var_check = ~isempty(find(strcmp(ElastEditNames,ElastoDynList{i}),1));
            if var_check == 0
                error(['Invalid ED.input variable, "',ElastoDynList{i},'" does not exist. Check your spelling',...
                    ' or if that variable exists in the current OpenFAST version. If it does',...
                    'add it to the file "fast_input_variable_names.m".'])
            end
        end
        
        % Create the appropriate cell arrays for the Af_EditElast function.
%         ElastoEditLine = strrep(ElastoDynList,'ED_','');
        ElastoEditLine = ElastoDynList;
        ElastoEditValue = cell(length(ElastoEditLine));
        for i=1:1:length(ElastoEditLine)
            ElastoEditValue{i} = eval(['ED.',ElastoDynList{i}]);
        end
        
        Af_EditElast(ElastoEditLine,ElastoEditValue,Name_ED_New,Name_ED_Def);
    else
        % If there are no edits, then the Master input file will be copied.
        ElastoEditLine = {};
        ElastoEditValue = {};
        Af_EditElast(ElastoEditLine,ElastoEditValue,Name_ED_New,Name_ED_Def);
    end
elseif FA_CompElast == 0
    disp('ElastoDyn not enabled in the .fst file.');
else
    error('Not a valid option for FA.CompElast.');    
end

clear ElastoDynList ElastoEditLine ElastoEditValue i var_check

%% ServoDyn
% ServoDynList = who('SD_*');

if ~isfield(FA,'CompServo')
    FA.CompServo = 1;
    default_FAST = 1;
end

if ~exist('SD','var')
    SD = struct();
    default_servo = 1;
end

ServoDynList = fieldnames(SD);

if FA.CompServo == 1
    if ~isempty(ServoDynList)
        % If there are user-defined SD.input values, modify the master ServoDyn.ipt file.
        for i=1:1:length(ServoDynList)
            var_check = ~isempty(find(strcmp(ServoEditNames,ServoDynList{i}),1));
            if var_check == 0
                error(['Invalid SD.input variable, "',ServoDynList{i},'" does not exist. Check your spelling',...
                    ' or if that variable exists in the current OpenFAST version. If it does',...
                    'add it to the file "fast_input_variable_names.m".'])
            end
        end
        
        % Create the appropriate cell arrays for the Af_EditServo function.
%         ServoEditLine = strrep(ServoDynList,'SD_','');
        ServoEditLine = ServoDynList;
        ServoEditValue = cell(length(ServoEditLine));
        for i=1:1:length(ServoEditLine)
            ServoEditValue{i} = eval(['SD.',ServoDynList{i}]);
        end
        
        Af_EditServo(ServoEditLine,ServoEditValue,Name_SD_New,Name_SD_Def);
    else
        % If there are no edits, then the Master input file will be copied.
        ServoEditLine = {};
        ServoEditValue = {};
        Af_EditServo(ServoEditLine,ServoEditValue,Name_SD_New,Name_SD_Def);
    end
elseif FA_CompServo == 0
    disp('ServoDyn not enabled in the .fst file.');
else
    error('Not a valid option for FA.CompServo.');    
end

clear ServoDynList ServoEditLine ServoEditValue i var_check
if exist('default_servo','var')
    clear default_servo FA_CompServo
end

%% InflowWind
% InflowWindList = who('IW_*');

if ~isfield(FA,'CompInflow')
    FA.CompInflow = 1;
    default_FAST = 1;
end

if ~exist('IW','var')
    IW = struct();
    default_inflow = 1;
end

InflowWindList = fieldnames(IW);

if FA.CompInflow == 1
    if ~isempty(InflowWindList)
        % If there are user-defined IW.input values, modify the master InflowWind.ipt file.
        for i=1:1:length(InflowWindList)
            var_check = ~isempty(find(strcmp(InflowEditNames,InflowWindList{i}),1));
            if var_check == 0
                error(['Invalid IW.input variable, "',InflowWindList{i},'" does not exist. Check your spelling',...
                    ' or if that variable exists in the current OpenFAST version. If it does',...
                    'add it to the file "fast_input_variable_names.m".'])
            end
        end
        
        % Create the appropriate cell arrays for the Af_EditInflow function.
%         InflowEditLine = strrep(InflowWindList,'IW_','');
        InflowEditLine = InflowWindList;
        InflowEditValue = cell(length(InflowEditLine));
        for i=1:1:length(InflowEditLine)
            InflowEditValue{i} = eval(['IW.',InflowWindList{i}]);
        end
        
        Af_EditInflow(InflowEditLine,InflowEditValue,Name_IW_New,Name_IW_Def);
    else
        % If there are no edits, then the Master input file will be copied.
        InflowEditLine = {};
        InflowEditValue = {};
        Af_EditInflow(InflowEditLine,InflowEditValue,Name_IW_New,Name_IW_Def);
    end
elseif FA.CompInflow == 0
    disp('InflowWind not enabled in the .fst file.');
else
    error('Not a valid option for FA.CompInflow.');    
end

clear InflowWindList InflowEditLine InflowEditValue i var_check
if exist('default_inflow','var')
    clear default_inflow FA_CompInflow
end

%% BeamDyn
% BeamDynList = who('BD_*');

if ~exist('BD','var')
    BD = struct();
    default_beamdyn = 1;
end

BeamDynList = fieldnames(BD);

if FA.CompElast == 2
    if ~isempty(BeamDynList)
        % If there are user-defined BD.input values, modify the master BeamDyn.ipt file.
        for i=1:1:length(BeamDynList)
            var_check = ~isempty(find(strcmp(BeamEditNames,BeamDynList{i}),1));
            if var_check == 0
                error(['Invalid BD.input variable, "',BeamDynList{i},'" does not exist. Check your spelling',...
                    ' or if that variable exists in the current OpenFAST version. If it does',...
                    'add it to the file "fast_input_variable_names.m".'])
            end
        end
        
        % Create the appropriate cell arrays for the Af_EditBeam function.
%         BeamEditLine = strrep(BeamDynList,'BD_','');
        BeamEditLine = BeamDynList;
        BeamEditValue = cell(length(BeamEditLine));
        for i=1:1:length(BeamEditLine)
            BeamEditValue{i} = eval(['BD.',BeamDynList{i}]);
        end
        
        Af_EditBeam(BeamEditLine,BeamEditValue,Name_BD_New,Name_BD_Def,TemplateDir);
    else
        % If there are no edits, then the Master input file will be copied.
        BeamEditLine = {};
        BeamEditValue = {};
        Af_EditBeam(BeamEditLine,BeamEditValue,Name_BD_New,Name_BD_Def,TemplateDir);
    end
elseif FA.CompElast == 1
    disp('BeamDyn not enabled in the .fst file.');    
end

clear BeamDynList BeamEditLine BeamEditValue i var_check
if exist('default_elast','var')
    clear default_elast FA_CompElast
end
%% OpenFAST

FASTList = fieldnames(FA);

if ~isempty(FASTList)
    % If there are user-defined FA.input values, modify the master FAST.fst file.
    for i=1:1:length(FASTList)
        var_check = ~isempty(find(strcmp(FastEditNames,FASTList{i}),1));
        if var_check == 0
            error(['Invalid FA.input variable, "',FASTList{i},'" does not exist. Check your spelling',...
                ' or if that variable exists in the current OpenFAST version. If it does',...
                'add it to the file "fast_input_variable_names.m".'])
        end
    end
    
    % Create the appropriate cell arrays for the Af_EditFAST function.
%     FASTEditLine = strrep(FASTList,'FA_','');
    FASTEditLine = FASTList;
    FASTEditValue = cell(length(FASTEditLine));
    for i=1:1:length(FASTEditLine)
        FASTEditValue{i} = eval(['FA.',FASTList{i}]);
    end
    
    Af_EditFast(FASTEditLine,FASTEditValue,Name_FAST_New,Name_FAST_Def);
else
    % If there are no edits, then the Master input file will be copied.
    FASTEditLine = {};
    FASTEditValue = {};
    Af_EditFast(FASTEditLine,FASTEditValue,Name_FAST_New,Name_FAST_Def);
end

clear FASTList FASTEditLine FASTEditValue i var_check AeroEditNames BeamEditNames ElastEditNames ...
      FastEditNames InflowEditNames Name_AD* Name_BD* Name_ED* Name_FAST* Name_IW* Name_SD* ServoEditNames

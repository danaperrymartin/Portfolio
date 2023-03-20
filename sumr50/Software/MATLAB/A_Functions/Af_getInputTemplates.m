function Defs = Af_getInputTemplates(turbineString)
global A_CD

if ~exist(fullfile(A_CD,'FAST8_IF','TemplateTable.mat'))
    disp('TemplateTable.mat not in FAST8_IF...running Af_GetFileTable.m');
    Af_GetFileTable;
    
else
    load(fullfile(A_CD,'FAST8_IF','TemplateTable'));
end

ModelInd = strcmp(ModelString,turbineString);

if sum(ModelInd) == 0
    disp('Model not properly loaded from Google Sheet, running Af_GetFileTable.m')
    Af_GetFileTable;
    ModelInd = strcmp(ModelString,turbineString);
    if sum(ModelInd) == 0
        disp('Still can''t find model, check sheet');
    end
end


temp = FAST_Template{ModelInd};
[~,Defs.FAST] = fileparts(temp);

temp = ED_Template{ModelInd};
[~,Defs.ED] = fileparts(temp);

temp = AD14_Template{ModelInd};
[~,Defs.AD14] = fileparts(temp);

temp = AD15_Template{ModelInd};
[~,Defs.AD15] = fileparts(temp);

temp = SD_Template{ModelInd};
[~,Defs.SD] = fileparts(temp);

temp = IW_Template{ModelInd};
[~,Defs.IW] = fileparts(temp);

temp = BD_Template{ModelInd};
[~,Defs.BD] = fileparts(temp);

temp = Tower_Template{ModelInd};
[~,Defs.Twr] = fileparts(temp);

Defs.ED_Blade = ED_BladeFile{ModelInd};

Defs.AD15_Blade = AD15_BladeFile{ModelInd};

Defs.BD_Blade   = BD_BladeFile{ModelInd};


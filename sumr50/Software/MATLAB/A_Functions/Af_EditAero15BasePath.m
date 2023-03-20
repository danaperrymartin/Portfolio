function Af_EditAero15BasePath(defname,basePath)

%% Open Files
newname = [defname,'_temp'];
fidDefault  = fopen([defname,'.ipt'],'r');
if fidDefault==-1
    error(['Error: ', defname, '.ipt not found.  Note: you do not need to end string with .ipt']);
end
fidNew      = fopen([newname,'.ipt'],'w+');
if fidNew==-1
    error(['Error: ', newname, '.ipt not found.  Note: you do not need to end string with .ipt']);
end

%% If Edits are empty, copy file over and close

% Copy Master file lines over and modify values
while 1
    tline = fgetl(fidDefault);
    
    if ~ischar(tline)
        %Break @ end of file
        break;
    end
    
    editedLine = false;
    
    % Edit lines with user-defined values from MATLAB script
    lineInd = strfind(tline,'Master');
    
    if lineInd
        editedLine = true;
        tlineMod = tline(lineInd:end);
        
        edits = ['"',basePath];
        
        fprintf(fidNew,'%s%s\n',edits,[filesep,tlineMod]);
    end
    
    if ~editedLine
        % If we don't need to edit, print original line
        fprintf(fidNew,'%s\n',tline);
    end
end


%% Close Files

fclose('all');

copyfile([newname,'.ipt'],[defname,'.ipt'],'f');
delete([newname,'.ipt']);

% disp(['Status: ', newname,'.ipt created with ',num2str(length(edits)),' changed input(s).']);


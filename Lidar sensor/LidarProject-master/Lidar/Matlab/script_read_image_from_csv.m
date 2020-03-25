clear;
close all;
clc;

infiniteDistance = -1;
files_extension = 'csv';
folderPath = '../Debug_Images';
folder_search = sprintf('%s/*.%s',folderPath,files_extension);
files_dir = dir(folder_search);
num_of_files = length(files_dir);
for i=1:num_of_files
    file_base_name = 'matlabImage_';
    filePath = sprintf('%s/%s%d.%s',folderPath,file_base_name,i,files_extension);
    tableDistances = readtable(filePath);
    imgDistances = table2array(tableDistances);
    [rows,cols] = find(imgDistances == infiniteDistance);

    % sortedVals = sort(imgDistances(:));
    [unique_distances, ia, ic] = unique(imgDistances,'sorted');
    unique_distances_without_infinite_distance = unique_distances;
    unique_distances_without_infinite_distance(unique_distances_without_infinite_distance==infiniteDistance)='';
    if (~isempty(unique_distances_without_infinite_distance))
        maxVal = max(unique_distances_without_infinite_distance);
        minVal = min(unique_distances_without_infinite_distance);
        imgDistancesZeroToOne = (imgDistances-minVal)./(maxVal-minVal);
        idx = sub2ind(size(imgDistances), rows, cols);
        imgDistancesZeroToOne(idx) = 0;

        figure;
        imshow(imgDistancesZeroToOne);
    else
        figure;
        imshow(zeros(size(imgDistances)));
    end
   
end


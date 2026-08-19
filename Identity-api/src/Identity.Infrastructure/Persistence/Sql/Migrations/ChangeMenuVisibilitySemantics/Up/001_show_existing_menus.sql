UPDATE menus AS child
INNER JOIN menus AS parent
    ON parent.Id = child.ParentId
    AND parent.ApplicationId = child.ApplicationId
SET child.IsVisible = 0
WHERE parent.Code = 'Identity'
    AND parent.IsDeleted = 0
    AND child.IsDeleted = 0;

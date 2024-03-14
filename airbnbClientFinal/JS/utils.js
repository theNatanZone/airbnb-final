function getPropertyFromSessionStorage(propertyName) {
    var userDetails = sessionStorage.getItem('userDetails');
    
    if (userDetails) {
        try {
            var userDetailsObj = JSON.parse(userDetails);
            
            if (userDetailsObj.hasOwnProperty(propertyName)) {

                if (propertyName === 'FullName') {
                    var fullName = userDetailsObj[propertyName];
                    
                    if (typeof fullName === 'string') {
                        var fullNameParts = fullName.split(' ');
                        
                        if (fullNameParts.length >= 2) {

                            return {
                                FirstName: fullNameParts[0],
                                FamilyName: fullNameParts.slice(1).join(' ')
                            };
                        } else {
                            console.error('Full name is not in the correct format');
                            return null;
                        }
                    } else {
                        console.error('The specified property is not a string');
                        return null;
                    }
                } else {

                    return userDetailsObj[propertyName];
                }
            } else {
                console.error('The specified property not found in userDetails');
                return null;
            }
        } catch (error) {
            console.error('Error parsing userDetails:', error);
            return null;
        }
    } else {
        console.error('userDetails not found in session storage');
        return null;
    }
}

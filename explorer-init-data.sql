DELETE FROM stakeholders."People";
DELETE FROM stakeholders."Users";



INSERT INTO stakeholders."Users"(
	"Id", "Username", "Password", "Role", "IsActive", "IsVerified")
	VALUES (1, 'daks', '$2a$12$j5tA8zfzm0UZv.6VObLHne8QOMHeduDlxXlejvrGoeP6Do9rWeKIu', 0, true, true);

INSERT INTO stakeholders."People"(
	"Id", "UserId", "Name", "Surname", "ProfilePictureUrl", "Biography", "Motto", "Email")
	VALUES (1, 1, 'daks', 'daks', '', '', '', '');
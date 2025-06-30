create database if not exists forum character set utf8mb4 collate utf8mb4_unicode_ci;
use forum;

create table if not exists Users(
    Email varchar(256) not null primary key,
    Password char(88) not null,
    Username varchar(50) not null,
    CreatedAt datetime not null,
    Biography varchar(500) not null
);

create table if not exists Entries(
    Author varchar(256) not null,
    CreatedAt datetime not null,
    Title varchar(100) not null,
    Text mediumtext not null,
    primary key (Author, CreatedAt),
    foreign key (Author) references Users(Email)
    on update cascade
    on delete cascade
);

drop procedure if exists register_user;
drop procedure if exists change_username;
drop procedure if exists edit_biography;
drop procedure if exists create_entry;
drop procedure if exists delete_entry;

delimiter $$

create procedure register_user(_email varchar(256), _password char(88), _username varchar(50))
begin
    if (exists(select 1 from Users where Username = _username)) then
        signal sqlstate '45000' set message_text = 'This username is already taken';
    end if;

    insert into Users (Email, Password, Username, CreatedAt, Biography)
    values (_email, _password, _username, current_timestamp, '-');
end $$

create procedure change_username(_email varchar(256), _username varchar(50))
begin
    update Users
    set Username = _username
    where Email = _email;
end $$

create procedure edit_biography(_email varchar(256), _biography varchar(500))
begin
    update Users
    set Biography = ifnull(_biography, '-')
    where Email = _email;
end $$

create procedure create_entry(_email varchar(256), _title varchar(100), _text mediumtext)
begin
    insert into Entries (Author, CreatedAt, Title, Text)
    values (_email, current_timestamp, _title, _text);
end $$

create procedure delete_entry(_email varchar(256), _created_at datetime)
begin
    delete from Entries
    where Email = _email and CreatedAt = _created_at;
end $$
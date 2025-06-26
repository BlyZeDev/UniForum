create database if not exists forum character set utf8mb4 collate utf8mb4_unicode_ci;
use forum;

create table if not exists Users(
    Email varchar(256) not null primary key,
    Password binary(48) not null,
    Username varchar(50) not null,
    CreatedAt datetime not null,
    Biography varchar(500) not null,
    IsDeleted boolean not null
);

create table if not exists Topics(
    Creator varchar(256) not null,
    CreatedAt datetime not null,
    Title varchar(100) not null,
    primary key (Creator, Title),
    foreign key (Creator) references Users(Email)
    on update cascade
    on delete cascade
);

create table if not exists Entries(
    Author varchar(256) not null,
    CreatedAt datetime not null,
    Title varchar(100) not null,
    Text mediumtext not null,
    TopicCreator varchar(256),
    TopicTitle varchar(100),
    primary key (Author, CreatedAt),
    foreign key (Author) references Users(Email)
    on update cascade
    on delete cascade,
    foreign key (TopicCreator, TopicTitle) references Topics(Creator, Title)
    on update cascade
    on delete set null
);

create table if not exists Likes(
    LikeUser varchar(256) not null,
    EntryAuthor varchar(256) not null,
    EntryCreatedAt datetime not null,
    primary key (LikeUser, EntryAuthor, EntryCreatedAt),
    foreign key (LikeUser) references Users(Email)
    on update cascade
    on delete cascade,
    foreign key (EntryAuthor, EntryCreatedAt) references Entries(Author, CreatedAt)
    on update cascade
    on delete cascade
);

drop procedure if exists register_user;
drop procedure if exists change_username;
drop procedure if exists edit_biography;
drop procedure if exists delete_user;
drop procedure if exists create_topic;
drop procedure if exists delete_topic;
drop procedure if exists create_entry;
drop procedure if exists edit_entry;
drop procedure if exists delete_entry;
drop procedure if exists like_entry;
drop procedure if exists unlike_entry;

delimiter $$

create procedure register_user(_email varchar(256), _password binary(64), _username varchar(50))
begin
    if (exists(select 1 from Users where Username = _username)) then
        signal sqlstate '45000' set message_text = 'This username is already taken';
    end if;

    insert into Users (Email, Password, Username, CreatedAt, Biography, IsDeleted)
    values (_email, _password, _username, current_timestamp, '-', false);
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

create procedure delete_user(_email varchar(256))
begin
    update Users
    set Username = '[deleted]', Password = 0x00, Biography = '-', IsDeleted = true
    where Email = _email;
end $$

create procedure create_topic(_email varchar(256), _title varchar(100))
begin
    insert into Topics (Creator, CreatedAt, Title)
    values (_email, current_timestamp, _title);
end $$

create procedure delete_topic(_email varchar(256), _title varchar(100))
begin
    delete from Topics
    where Creator = _email and Title = _title;
end $$

create procedure create_entry(_email varchar(256), _title varchar(100), _text mediumtext, _topic_creator varchar(256), _topic_title varchar(100))
begin
    insert into Entries (Author, CreatedAt, Title, Text, TopicCreator, TopicTitle)
    values (_email, current_timestamp, _title, _text, _topic_creator, _topic_title);
end $$

create procedure edit_entry(_email varchar(256), _created_at datetime, _title varchar(100), _text mediumtext)
begin
    update Entries
    set Title = _title, Text = _text
    where Email = _email and CreatedAt = _created_at;
end $$

create procedure delete_entry(_email varchar(256), _created_at datetime)
begin
    delete from Entries
    where Email = _email and CreatedAt = _created_at;
end $$

create procedure like_entry(_like_user varchar(256), _entry_author varchar(256), _entry_created_at datetime)
begin
    insert into Likes (LikeUser, EntryAuthor, EntryCreatedAt)
    values (_like_user, _entry_author, _entry_created_at);
end $$

create procedure unlike_entry(_like_user varchar(256), _entry_author varchar(256), _entry_created_at datetime)
begin
    delete from Likes
    where LikeUser = _like_user and EntryAuthor = _entry_author and EntryCreatedAt = _entry_created_at;
end $$
using OngProject.Core.Models.DTOs;
using OngProject.Entities;

namespace OngProject.Core.Interfaces
{
    public interface IEntityMapper
    {
        Organization OrganizationDtoForUploadtoOrganization(OrganizationDTOForUpload organizationDTOForUpload);
        OrganizationDTOForDisplay OrganizationToOrganizationDTOForDisplay(Organization organization);
        UserDTO UserToUserDto(User user);
        UserDetailDto UserToUserDetailDto(User user);
        User UserRegisterDtoToUser(UserRegisterDto dto);
        UserDtoForDisplay UserToUserDtoForDisplay(User user);
        SlideDtoForDisplay SlideToSlideDtoForDisplay(Slides slides);
        Slides SlideDtoForUploadToSlide(SlideDtoForUpload slideDto);
        ContactDTO ContactToContactDTO(Contacts contacts);
        Contacts ContactDTOToContact(ContactDTO contactsDto);
        CommentDtoForDisplay CommentToCommentDtoForDisplay(Comment comment);
        Comment CommentForRegisterToComment(CommentDtoForRegister commentDto);
        CategoryDtoForDisplay CategoryToCategoryDtoForDisplay(Category category);
        Category CategoryDtoForRegisterToCategory(CategoryDTOForRegister category);
        CategoryDTO CategoryToCategoryDTO(Category category);
        New NewDtoForUploadtoNew(NewDtoForUpload newDTO);
        NewDtoForDisplay NewtoNewDtoForDisplay(New newvar);
        Activities ActivityDTOForRegister(ActivityDTOForRegister dto);
        ActivityDTOForDisplay ActivityForActivityDTODisplay(Activities dto);
        TestimonialDTO TestimonialToTestimonialDTO(Testimonials testimonial);
        Testimonials TestimonialDTOToTestimonial(TestimonialDTO testimonialDTO);
        TestimonialDTODisplay TestimonialDTOToTestimonialDisplay(Testimonials testimonal);
        MemberDTORegister MemberToMemberDTO(Member member);
        MemberDTODisplay MemberToMemberDTODisplay(Member member);
        Member MemberDTORegisterToMember(MemberDTORegister memberDTO);
    }
}
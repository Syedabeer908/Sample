using WebApplication1.Common.Exceptions;
using WebApplication1.Common.Results;
using WebApplication1.Data;
using WebApplication1.DTOs.Control;
using WebApplication1.Entities;
using WebApplication1.Interfaces;
using WebApplication1.Mappers;

namespace WebApplication1.Services
{
    public class ControlService
    {
        private readonly IRepository<Control> _repo;
        private readonly ISoftRepository _softRepo;
        private readonly ControlMapper _mapper;
        public ControlService(IRepository<Control> repo, ISoftRepository softRepo)
        {
            _repo = repo;
            _softRepo = softRepo;
            _mapper = new ControlMapper();
        }

        private async Task<Control> CheckControlExistAndGet(Guid id)
        {
            var control = await _repo.GetByIdAsync(id);

            if (control == null)
                throw new NotFoundException($"Control with id {id} not found.");

            return control;
        }

        private async Task RunSoftService(Guid id, bool action, Guid actionBy)
        {
            var soft = new Soft();
            var values = soft.SoftValuesSetter(action, DateTime.UtcNow, actionBy);
            await _softRepo.SoftControlAsync(id, values);
        }

        public async Task<ResultT<List<ControlDto>>> GetAllAsync()
        {
            var controls = await _repo.GetAllAsync();
            var controlDtos = controls.Select(c => _mapper.ToDto(c)).ToList();
            return ResultT<List<ControlDto>>.Success(controlDtos);
        }

        public async Task<ResultT<ControlDto>> GetByIdAsync(Guid id)
        {
            var control = await CheckControlExistAndGet(id);
            return ResultT<ControlDto>.Success(_mapper.ToDto(control));
        }

        public async Task<ResultT<ControlDto>> AddAsync(Guid userId, CreateControlDto dto)
        {
            var control = _mapper.ToEntity(userId, dto);
            await _repo.AddAsync(control);

            var data = await CheckControlExistAndGet(control.ControlId);

            return ResultT<ControlDto>.Success(_mapper.ToDto(data));
        }

        public async Task<ResultT<ControlDto>> UpdateAsync(Guid id, Guid userId, UpdateControlDto dto)
        {
            var control = await CheckControlExistAndGet(id);
            _mapper.UpdateEntity(control, userId, dto);
            await _repo.UpdateAsync(control);
            return ResultT<ControlDto>.Success(_mapper.ToDto(control));
        }

        public async Task<Result> DeleteAsync(Guid id, Guid userId)
        {
            var control = await CheckControlExistAndGet(id);
            await RunSoftService(id, true, userId);
            return Result.Success();
        }

        public async Task<Result> RestoreAsync(Guid id, Guid userId)
        {
            var control = await CheckControlExistAndGet(id);
            await RunSoftService(id, false, userId);
            return Result.Success();
        }

        public async Task<ResultT<ControlDto>> PatchAsync(Guid id, Guid userId, PatchControlDto dto)
        {
            var control = await CheckControlExistAndGet(id);
            _mapper.PatchEntity(control, userId, dto);
            await _repo.UpdateAsync(control);
            return ResultT<ControlDto>.Success(_mapper.ToDto(control));
        }
    }
}

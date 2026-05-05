using WebApplication1.Common.Exceptions;
using WebApplication1.Common.Results;
using WebApplication1.Data;
using WebApplication1.DTOs.Risk;
using WebApplication1.Entities;
using WebApplication1.Interfaces;
using WebApplication1.Mappers;

namespace WebApplication1.Services
{
    public class RiskService
    {
        private readonly IRepository<Risk> _repo;
        private readonly ISoftRepository _softRepo;
        private readonly RiskMapper _mapper;
        public RiskService(IRepository<Risk> repo, ISoftRepository softRepo)
        {
            _repo = repo;
            _softRepo = softRepo;
            _mapper = new RiskMapper();
        }

        private async Task<Risk> CheckRiskExistAndGet(Guid id)
        {
            var risk = await _repo.GetByIdAsync(id);

            if (risk == null)
                throw new NotFoundException($"Risk with id {id} not found.");

            return risk;
        }

        private async Task RunSoftService(Guid id, bool action, Guid actionBy)
        {
            var soft = new Soft();
            var values = soft.SoftValuesSetter(action, DateTime.UtcNow, actionBy);
            await _softRepo.SoftRiskAsync(id, values);
        }

        public async Task<ResultT<List<RiskDto>>> GetAllAsync()
        {
            var risks = await _repo.GetAllAsync();
            var riskDtos = risks.Select(r => _mapper.ToDto(r)).ToList();
            return ResultT<List<RiskDto>>.Success(riskDtos);
        }

        public async Task<ResultT<RiskDto>> GetByIdAsync(Guid id)
        {
            var risk = await CheckRiskExistAndGet(id);
            return ResultT<RiskDto>.Success(_mapper.ToDto(risk));
        }

        public async Task<ResultT<RiskDto>> AddAsync(Guid userId, CreateRiskDto dto)
        {
            var risk = _mapper.ToEntity(userId, dto);
            await _repo.AddAsync(risk);

            var data = await CheckRiskExistAndGet(risk.RiskId);

            return ResultT<RiskDto>.Success(_mapper.ToDto(data));
        }

        public async Task<ResultT<RiskDto>> UpdateAsync(Guid id, Guid userId, UpdateRiskDto dto)
        {
            var risk = await CheckRiskExistAndGet(id);
            _mapper.UpdateEntity(risk, userId, dto);
            await _repo.UpdateAsync(risk);
            return ResultT<RiskDto>.Success(_mapper.ToDto(risk));
        }

        public async Task<Result> DeleteAsync(Guid id, Guid userId)
        {
            var risk = await CheckRiskExistAndGet(id);
            await RunSoftService(id, true, userId);
            return Result.Success();
        }

        public async Task<Result> RestoreAsync(Guid id, Guid userId)
        {
            var risk = await CheckRiskExistAndGet(id);
            await RunSoftService(id, false, userId);
            return Result.Success();
        }

        public async Task<ResultT<RiskDto>> PatchAsync(Guid id, Guid userId, PatchRiskDto dto)
        {
            var risk = await CheckRiskExistAndGet(id);
            _mapper.PatchEntity(risk, userId, dto);
            await _repo.UpdateAsync(risk);
            return ResultT<RiskDto>.Success(_mapper.ToDto(risk));
        }
    }
}
